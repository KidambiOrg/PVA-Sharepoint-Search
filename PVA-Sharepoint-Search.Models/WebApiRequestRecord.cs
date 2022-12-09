namespace PVA_Sharepoint_Search.Models
{
    public record WebApiRequestRecord
    {
        public string? RecordId { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}