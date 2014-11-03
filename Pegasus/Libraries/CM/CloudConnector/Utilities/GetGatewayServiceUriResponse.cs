using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GetGatewayServiceUriResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="uri"></param>
        public GetGatewayServiceUriResponse(GetGatewayServiceUriResult result, Uri uri)
        {
            Result = result;
            Uri = uri;
        }

        /// <summary>
        /// 
        /// </summary>
        public GetGatewayServiceUriResult Result { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri Uri { get; private set; }
    }
}
