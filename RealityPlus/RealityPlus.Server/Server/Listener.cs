using RealityPlus.Server.DI;
using RealityPlus.Server.Interfaces;
using System.Net;

namespace RealityPlus.Server.Server
{
    internal class Listener: IDisposable
    {
        private readonly HttpListener Server;
        private readonly IEnumerable<IServerController> Controllers;

        public Listener(IEnumerable<IServerController> controllers, Configuration configuration)
        {
            Controllers = controllers;
            Server = new HttpListener();
            Server.Prefixes.Add(configuration.BaseUrl);
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            Server.Start();
            WaitForRequest();
        }

        public void Stop()
        {
            Server.Stop();
        }

        public void WaitForRequest()
        {
            Server.BeginGetContext(new AsyncCallback(ProcessRequest), Server);
        }

        private void ProcessRequest(IAsyncResult result)
        {
            if (Server.IsListening)
            {
                var context = Server.EndGetContext(result);
                var request = context.Request;
                context.Response.Headers.Clear();
                context.Response.StatusCode = 200;
                Console.WriteLine($"{request.Url}");

                using (var stream = context.Response.OutputStream)
                {
                    using (var writer =  new StreamWriter(stream,  System.Text.Encoding.UTF8))
                    {
                        try
                        {
                            foreach (var controller in Controllers)
                            {
                                if (controller.HandleRequest(request, writer))
                                {
                                    context.Response.Headers.Add("Content-type", "application/json; charset=utf-8");
                                    break;
                                }
                            }
                        }
                        catch(Exception ex) 
                        {
                            Console.WriteLine(ex.Message);
                            writer.WriteLine(ex.Message);
                        }
                    }
                    stream.Flush();
                    stream.Close();
                }

                WaitForRequest();
            }
        }
    }
}
