using System;
using System.Collections.Generic;
using System.Text;

namespace TrainsCSharp.Core.Exceptions
{
    public class RouteNotFoundException : Exception
    {
        public string StandardRouteNotFoundUserMessage { get; private set; } = "NO SUCH ROUTE";
    }
}
