using ByondTopic.Response;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ByondTopic
{
    public class TopicSource
    {
        private string Server { get; }
        private ushort Port { get; }
        
        public TopicSource(string server, ushort port)
        {
            Server = server;
            Port = port;
        }

        /// <summary>
        /// Calls Topic on world with the provided command
        /// </summary>
        /// <param name="command">The command to request execution for</param>
        /// <returns>A dictionary of keys and values returned from the server</returns>
        public QueryResponse Query(string command)
        {
            // Create request
            using var memStream = new MemoryStream();
            var binWtr = new BinaryWriter(memStream);
            binWtr.Write((byte)0x00);
            binWtr.Write((byte)0x83);
            binWtr.Write(ReverseBytes((ushort)(command.Length + 6)));
            for (int i = 0; i < 5; i++)
            {
                binWtr.Write((byte)0x00);
            }
            binWtr.Write(Encoding.UTF8.GetBytes(command));
            binWtr.Write((byte)0x00);

            // Write to stream / Send to server
            using var client = new TcpClient(Server, Port);
            NetworkStream stream = client.GetStream();
            stream.Write(memStream.GetBuffer(), 0, (int)binWtr.BaseStream.Length);

            // Validate response
            var binRdr = new BinaryReader(stream);
            if (binRdr.BaseStream.Length < 2)
            {
                throw new InvalidResponseException($"Invalid response, returned too few bytes ({binRdr.BaseStream.Length})");
            }
            else if (binRdr.ReadByte() != 0x00 || binRdr.ReadByte() != 0x83) // invalid format
            {
                throw new InvalidResponseException("Invalid response, cannot determine response type from first two bytes.");
            }

            // Determine response size
            ushort responseSize = ReverseBytes(binRdr.ReadUInt16());

            // Determine response type
            ResponseType responseType = ResponseType.Unknown;
            switch (binRdr.ReadByte())
            {
                case 0x2a:
                    responseType = ResponseType.Float;
                    break;
                case 0x06:
                    responseType = ResponseType.ASCII;
                    break;
            }

            // Create response object
            return responseType == ResponseType.Float 
                ? (QueryResponse)new FloatQueryResponse(stream) 
                : new TextQueryResponse(stream, responseSize);
        }

        /// <summary>
        /// Attempts to convert the result of a provided command's JSON output to an object
        /// </summary>
        /// <remarks>
        /// The result of the command should be JSON data, otherwise this will throw exceptions
        /// </remarks>
        /// <typeparam name="T">The type that the JSON data represents</typeparam>
        /// <param name="command">The command to request execution for</param>
        /// <param name="propertyNameCaseInsensitive">Operator controlling if property names are case sensitive</param>
        /// <returns>The deserialized JSON's object</returns>
        public T QueryJson<T>(string command, bool propertyNameCaseInsensitive = true)
        {
            // Verify that data is textual
            var data = Query(command);
            if (data.ResponseType != ResponseType.ASCII)
            {
                throw new InvalidResponseException($"Cannot convert non-ASCII response to JSON. (Response type: {data.ResponseType})");
            }

            return JsonSerializer.Deserialize<T>(data.AsText.ToString(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = propertyNameCaseInsensitive
            });
        }

        /// <summary>
        /// Reverses the order of bytes for a ushort to be in big endian order
        /// </summary>
        /// <remarks>
        /// Required for transmitting size to BYOND
        /// </remarks>
        /// <param name="value">The value to reverse the bytes of</param>
        /// <returns>The value with bytes reversed</returns>
        internal static ushort ReverseBytes(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }
    }
}
