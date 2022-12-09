using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVA_Sharepoint_Search.Models
{
    public record WebApiSkillRequest
    {
        public List<WebApiRequestRecord> Values { get; set; } = new List<WebApiRequestRecord>();
    }
}