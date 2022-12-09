using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PVA_Sharepoint_Search.Models;

namespace PVASPOSearch.CustomVision.Functions
{
    public class DetectBinary
    {
        private readonly ILogger _logger;

        public DetectBinary(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DetectBinary>();
        }

        [Function("DetectBinary")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string predictionKey = Environment.GetEnvironmentVariable("CustomVision-Prediction-Key");
            string projectId = Environment.GetEnvironmentVariable("CustomVision-ProjectId");
            string IterationId = Environment.GetEnvironmentVariable("CustomVision-IterationId");
            string predictionEndpoint = string.Format(Environment.GetEnvironmentVariable("CustomVision-Url"), projectId, IterationId);

            string skillName = executionContext.FunctionDefinition.Name;
            string jsonRequest = await new StreamReader(req.Body).ReadToEndAsync();

            int maxPages = 1;
            double threshold = 0.5;

            WebApiSkillRequest docs = JsonConvert.DeserializeObject<WebApiSkillRequest>(jsonRequest);
            IEnumerable<WebApiRequestRecord> requestRecords = docs.Values;

            WebApiSkillResponse skillResponse = await ProcessRequestRecordsAsync(skillName, requestRecords,
            async (inRecord, outRecord) =>
            {
                var pages = inRecord.Data["pages"] as JArray;
                var tags = new List<string>();
                var predictions = new List<object>();

                _logger.LogInformation($"Processing {inRecord.RecordId}");

                foreach (var page in pages.Take(maxPages))
                {
                    if (!IsBase64(page.ToString()))
                    {
                        _logger.LogInformation($"File is not base 64");
                        tags.AddRange(Enumerable.Empty<string>());
                    }
                    else
                    {
                        var pageBinaryData = Convert.FromBase64String(page.ToString());
                        var pagePredictions = await GetPredictionsForImageAsync(pageBinaryData, predictionEndpoint, predictionKey);

                        var analyzeResult = JsonConvert.DeserializeObject<AnalyzeResult>(pagePredictions);
                        var pageTags = analyzeResult.Predictions.Where(p => p.Probability >= threshold).Select(p => p.TagName).Distinct();
                        tags.AddRange(pageTags);
                        _logger.LogInformation($"Tags: {pageTags.FirstOrDefault()}");
                    }
                }

                outRecord.Data["tags"] = tags.Distinct().ToArray();
                return outRecord;
            });

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(skillResponse);

            return response;
        }

        private static async Task<string> GetPredictionsForImageAsync(byte[] imageData, string predictionUrl, string predictionKey)
        {
            var client = HttpClientFactory.Create();
            client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(predictionUrl, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<WebApiSkillResponse> ProcessRequestRecordsAsync(string functionName, IEnumerable<WebApiRequestRecord> requestRecords, Func<WebApiRequestRecord, WebApiResponseRecord, Task<WebApiResponseRecord>> processRecord)
        {
            WebApiSkillResponse response = new WebApiSkillResponse();

            foreach (WebApiRequestRecord inRecord in requestRecords)
            {
                WebApiResponseRecord outRecord = new WebApiResponseRecord() { RecordId = inRecord.RecordId };

                try
                {
                    outRecord = await processRecord(inRecord, outRecord);
                }
                catch (Exception e)
                {
                    outRecord.Errors.Add(new WebApiErrorWarningContract() { Message = $"{functionName} - Error processing the request record : {e.ToString()}" });
                }
                response.Values.Add(outRecord);
            }

            return response;
        }

        private bool IsBase64(string base64String)
        {
            try
            {
                if (!base64String.Equals(Convert.ToBase64String(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(Convert.FromBase64String(base64String)))), StringComparison.InvariantCultureIgnoreCase) & !System.Text.RegularExpressions.Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,2}$"))
                {
                    return false;
                }
                else if ((base64String.Length % 4) != 0 || string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0 || base64String.Contains(" ") || base64String.Contains(Constants.vbTab) || base64String.Contains(Constants.vbCr) || base64String.Contains(Constants.vbLf))
                {
                    return false;
                }
                else return true;
            }
            catch (FormatException ex)
            {
                return false;
            }
        }
    }
}