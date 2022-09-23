using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVASharePointSearch.Business;
using PVASharePointSearch.Models;

namespace PVASharePointSearch.API.Functions
{
    public class BlobMetadaFunction
    {
        private readonly ILogger _logger;

        public BlobMetadaFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BlobMetadaFunction>();
        }

        [Function("BlobMetada")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            RequestModel requestModel = JsonConvert.DeserializeObject<RequestModel>(await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false));

            await new Manager(_logger).PostBlobMetadata(requestModel).ConfigureAwait(false);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Blob updated. ");

            return response;
        }
    }
}