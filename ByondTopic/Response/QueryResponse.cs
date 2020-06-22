using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByondTopic.Response
{
    public abstract class QueryResponse
    {
        /// <summary>
        /// The type of response as determined during the network connection
        /// </summary>
        public virtual ResponseType ResponseType { get; }
        /// <summary>
        /// Attempts to cast this QueryResponse to a TextQueryResponse, returns null if not possible
        /// </summary>
        public TextQueryResponse AsText => this as TextQueryResponse;
        /// <summary>
        /// Attempts to cast this QueryResponse to a FloatQueryResponse, returns null if not possible
        /// </summary>
        public FloatQueryResponse AsFloat => this as FloatQueryResponse;
    }
}
