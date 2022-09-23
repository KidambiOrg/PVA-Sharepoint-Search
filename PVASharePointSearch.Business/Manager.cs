using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using PVASharePointSearch.Models;
using System.Linq;
using System.Reflection.Metadata;

namespace PVASharePointSearch.Business
{
    public class Manager
    {
        private readonly ILogger _logger;

        public Manager(ILogger logger)
        {
            _logger = logger;
        }

        public async Task PostBlobMetadata(RequestModel requestModel)
        {
            _logger.LogInformation($"Getting blob references for '{requestModel.BlobName}'");

            BlobContainerClient containerClient = new BlobContainerClient(Environment.GetEnvironmentVariable("Document-Storage-Account"), Environment.GetEnvironmentVariable("Document-Container"));

            BlobClient blobClient = containerClient.GetBlobClient(requestModel.BlobName);

            //Add metadata
            IDictionary<string, string> metadata = new Dictionary<string, string>();

            _logger.LogInformation($"Adding metadata");
            foreach (RequestModelMetadata item in requestModel.Metadata)
            {
                // Add metadata to the dictionary by calling the Add method
                metadata.Add(item.Key, item.Value);
            }

            // Set the blob's metadata.
            _logger.LogInformation($"Updating blob metadata");
            await blobClient.SetMetadataAsync(metadata).ConfigureAwait(false);
        }
    }
}