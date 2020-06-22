using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ByondTopic.Response
{
    public class TextQueryResponse : QueryResponse
    {
        public override ResponseType ResponseType => ResponseType.ASCII;
        public string Response { get; }
        public Dictionary<string, string> AsDictionary => ConvertToDictionary();

        internal TextQueryResponse(Stream raw, int dataLength)
        {
            var binRdr = new BinaryReader(raw);
            Response = Encoding.ASCII.GetString(binRdr.ReadBytes(dataLength));
        }

        private Dictionary<string, string> ConvertToDictionary()
        {
            var qs = HttpUtility.ParseQueryString(Response);
            return qs
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
            return Response;
        }
    }
}
