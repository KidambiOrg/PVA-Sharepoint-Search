using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PVASharePointSearch.API.Middleware
{
    internal class CheckAPIKeyHeaderValidation : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var headerString = context.BindingContext.BindingData["Headers"] as string;

            Dictionary<string, string> headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headerString);
            if (!headers.ContainsKey("x-functions-key") || string.IsNullOrEmpty(headers["x-functions-key"].ToString()))
            {
                //If no file is attached, return bad request.
                string errorMessage = "Function Key header is missing and/or is empty. Add 'x-function-key' header with appropriate value and resed the request..";
                await SendErrorResponse(context, errorMessage, HttpStatusCode.BadRequest).ConfigureAwait(false);
            }
            else
            {
                await next(context).ConfigureAwait(false);
            }
        }

        private async Task SendErrorResponse(FunctionContext context, string errorMessage, HttpStatusCode statusCode)
        {
            var httpReqData = await context.GetHttpRequestDataAsync();
            if (httpReqData != null)
            {
                HttpResponseData newHttpResponse = httpReqData.CreateResponse();
                await newHttpResponse.WriteAsJsonAsync(new { ErrorStatus = errorMessage }, statusCode);

                // Update invocation result.
                context.GetInvocationResult().Value = newHttpResponse;
            }
        }
    }
}