using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ByondTopic.Response
{
    /// <summary>
    /// Contains an ASCII response from a BYOND world topic query
    /// </summary>
    public class TextQueryResponse : QueryResponse
    {
        public override ResponseType ResponseType => ResponseType.ASCII;
        /// <summary>
        /// The text content of the response
        /// </summary>
        public string Response { get; }
        /// <summary>
        /// Attempts to convert the text content to a dictionary
        /// </summary>
        public Dictionary<string, string> AsDictionary => ConvertToDictionary();

        internal TextQueryResponse(Stream raw, int dataLength)
        {
            using var binRdr = new BinaryReader(raw);
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
