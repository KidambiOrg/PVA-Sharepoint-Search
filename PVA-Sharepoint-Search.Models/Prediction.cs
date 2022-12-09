using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVA_Sharepoint_Search.Models
{
    public record Prediction
    {
        public double Probability { get; set; }
        public string? TagName { get; set; }

        public override string ToString()
        {
            return $"tag: {TagName}, probability {Probability}";
        }
    }
}