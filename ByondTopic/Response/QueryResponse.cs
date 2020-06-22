using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByondTopic.Response
{
    public abstract class QueryResponse
    {
        public virtual ResponseType ResponseType { get; }
        public TextQueryResponse AsText => this as TextQueryResponse;
        public FloatQueryResponse AsFloat => this as FloatQueryResponse;
    }
}
