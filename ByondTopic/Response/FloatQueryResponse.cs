using System.IO;

namespace ByondTopic.Response
{
    public class FloatQueryResponse : QueryResponse
    {
        public override ResponseType ResponseType => ResponseType.Float;
        public float Response { get; }

        internal FloatQueryResponse(Stream raw)
        {
            var binRdr = new BinaryReader(raw);
            Response = ReverseBytes(binRdr.ReadUInt32());
        }

        internal static float ReverseBytes(float value)
        {
            uint v = (uint)value;
            return (v & 0xFF) << 24 | (v & 0xFF00) << 8 | (v & 0xFF0000) >> 8 | (v & 0xFF000000) >> 24;
        }
    }
}
