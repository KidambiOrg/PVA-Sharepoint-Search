using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PVASharePointSearch.Models
{
    public record RequestModelMetadata
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}