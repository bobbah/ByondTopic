using System;
using System.Collections.Generic;
using System.Text;

namespace ByondTopic.Response
{
    /// <summary>
    /// Represents a null type response from BYOND.
    /// </summary>
    class NullQueryResponse : QueryResponse
    {
        public override ResponseType ResponseType => ResponseType.Null;

        public override string ToString()
        {
            return null;
        }
    }
}
