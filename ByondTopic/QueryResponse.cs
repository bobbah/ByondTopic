using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByondTopic
{
    public class QueryResponse
    {
        private string Raw { get; }
        public Dictionary<string, string> AsDictionary => ConvertToDictionary();


        internal QueryResponse(string raw)
        {
            Raw = raw;
        }

        private Dictionary<string, string> ConvertToDictionary()
        {
            var qs = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(Raw.Substring(5)));
            return HttpUtility.ParseQueryString(Raw)
                .AllKeys
                .Aggregate(new Dictionary<string, string>(), (Dictionary<string, string> seed, string key) =>
                {
                    if (key != null)
                    {
                        seed.Add(key, qs[key]);
                    }
                    return seed;
                }
            );
        }

        public override string ToString()
        {
            return Raw;
        }
    }
}
