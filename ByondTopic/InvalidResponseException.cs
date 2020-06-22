using System;
using System.Collections.Generic;
using System.Text;

namespace ByondTopic
{
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException()
        {
        }

        public InvalidResponseException(string message)
            : base(message)
        {
        }

        public InvalidResponseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
