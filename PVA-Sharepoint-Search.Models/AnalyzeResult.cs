using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVA_Sharepoint_Search.Models
{
    public record AnalyzeResult
    {
        public Prediction[]? Predictions { get; set; }
    }
}