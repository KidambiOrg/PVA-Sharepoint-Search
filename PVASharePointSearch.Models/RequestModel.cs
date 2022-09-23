namespace PVASharePointSearch.Models
{
    public record RequestModel
    {
        public string BlobName { get; set; }
        public IEnumerable<RequestModelMetadata> Metadata { get; set; }
    }
}