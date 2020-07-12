using ByondTopic.Response;
using System;
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
            using var binWtr = new BinaryWriter(memStream);
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
            NetworkStream stream = null;
            try
            {
                using var client = new TcpClient(Server, Port);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                throw new InvalidResponseException("Invalid response, unable to make connection to server. See inner exception for details.", ex);
            }
            stream.Write(memStream.GetBuffer(), 0, (int)binWtr.BaseStream.Length);

            // Validate response
            using var binRdr = new BinaryReader(stream);
            try
            {
                if (binRdr.ReadByte() != 0x00 || binRdr.ReadByte() != 0x83) // invalid format
                {
                    throw new InvalidResponseException("Invalid response, cannot determine response type from first two bytes.");
                }
            }
            catch (EndOfStreamException ex)
            {
                throw new InvalidResponseException("Invalid response, response ended before it was possible to determine response type.", ex);
            }
            catch (IOException ex)
            {
                if (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                {
                    throw new InvalidResponseException("Invalid response, server forcibly closed the connection. This was likely due to an invalid command.", ex);
                }
                else
                {
                    throw ex;
                }
            }

            // Determine response size
            ushort responseSize = ReverseBytes(binRdr.ReadUInt16());

            // Determine response type
            byte responseType = binRdr.ReadByte();
            switch (responseType)
            {
                case 0x00:
                    return new NullQueryResponse();
                case 0x2a:
                    return new FloatQueryResponse(stream);
                case 0x06:
                    return new TextQueryResponse(stream, responseSize);
                default:
                    throw new InvalidResponseException($"Invalid response, unable to determine response type [{responseType}]");
            }
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
