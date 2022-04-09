using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestAPI.Models
{
    public class Monkey
    {
        public string Name { get; set; }
        public string Location { get; set; }

        // [JsonPropertyName("ImageUrl")]
        public string ImageUrl { get; set; }
    }
}