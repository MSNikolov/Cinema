using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.DataProcessor.ExportDto
{
    public class JsonExportCustomer
    {
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Balance")]
        public string Balance { get; set; }
    }
}
