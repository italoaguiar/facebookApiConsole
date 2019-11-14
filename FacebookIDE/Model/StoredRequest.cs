using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookIDE.Model
{
    public class StoredRequest
    {
        public APIHttpRequest HttpRequest { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] ValidUriParameters { get; set; }
        public string[] ValidHeaderParameters { get; set; }
        public string[] ValidBodyParameters { get; set; }
        public Uri DocumentationUri { get; set; }

        public StoredRequest Clone()
        {
            var j = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<StoredRequest>(j);
        }
    }
}
