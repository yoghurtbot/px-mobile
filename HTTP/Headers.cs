using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace px_mobile.HTTP
{
    public class Headers : Dictionary<string, string>
    {
        public Headers() : base(StringComparer.CurrentCultureIgnoreCase)
        {

        }

        public string ContentType => ContainsKey("content-type") ? this["content-type"] : "";

        public bool LocationHeaderContains(string search)
        {
            if (!this.ContainsKey("location")) return false;

            string locationHeader = this["location"];
            return locationHeader.Contains(search);
        }

        public string GetHeaderValue(string key)
        {
            return this.ContainsKey(key) ? this[key] : "";
        }
    }
}
