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

            // Get response
            var response = new byte[4096];
            var bytesRead = stream.Read(response, 0, response.Length);
            var responseData = Encoding.ASCII.GetString(response, 5, bytesRead - 6);

            // Create response object
            return new QueryResponse(responseData);
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
            var data = Query(command).ToString();
            return JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions()
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
        private static ushort ReverseBytes(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }
    }
}
