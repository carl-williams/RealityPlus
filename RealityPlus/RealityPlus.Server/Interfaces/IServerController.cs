
using System.Net;

namespace RealityPlus.Server.Interfaces
{
    internal interface IServerController
    {
        public bool HandleRequest(HttpListenerRequest request, StreamWriter response);
    }
}
