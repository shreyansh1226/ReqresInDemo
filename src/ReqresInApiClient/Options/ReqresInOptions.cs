using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqresInApiClient.Options
{
    public class ReqresInOptions
    {
        public const string SectionName = "ReqresIn";
        public string BaseUrl { get; set; } = "https://reqres.in/api/";
        public string ApiKey { get; set; } 
        public int CacheDurationMinutes { get; set; } = 5;
        public int RetryCount { get; set; } = 3;
    }
}
