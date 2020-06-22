using System.IO;

namespace ByondTopic.Response
{
    /// <summary>
    /// Contains a float response from a BYOND world topic query
    /// </summary>
    public class FloatQueryResponse : QueryResponse
    {
        public override ResponseType ResponseType => ResponseType.Float;
        /// <summary>
        /// The float value contained in the response
        /// </summary>
        public float Response { get; }

        internal FloatQueryResponse(Stream raw)
        {
            using var binRdr = new BinaryReader(raw);
            Response = binRdr.ReadSingle();
        }

        public override string ToString()
        {
            return Response.ToString();
        }
    }
}
