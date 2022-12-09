using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVA_Sharepoint_Search.Models
{
    public record WebApiResponseRecord
    {
        public string? RecordId { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public List<WebApiErrorWarningContract> Errors { get; set; } = new List<WebApiErrorWarningContract>();
        public List<WebApiErrorWarningContract> Warnings { get; set; } = new List<WebApiErrorWarningContract>();
    }
}