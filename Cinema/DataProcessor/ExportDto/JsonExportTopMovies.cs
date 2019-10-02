using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.DataProcessor.ExportDto
{
    public class JsonExportTopMovies
    {
        [JsonProperty("MovieName")]
        public string Name { get; set; }

        [JsonProperty("Rating")]
        public string Rating { get; set; }

        [JsonProperty("TotalIncomes")]
        public string Income { get; set; }

        [JsonProperty("Customers")]
        public List<JsonExportCustomer> Customers { get; set; } = new List<JsonExportCustomer>();
    }
}
