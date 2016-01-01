using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Owin;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Octgn.Server;

namespace Octgn.Client
{
    public class UIBackend : IDisposable
    {
        //internal static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string WebFolderPath { get; private set; }
        public string Path { get; private set; }
        public int Port { get; private set; }
        protected IDisposable WebHost { get; set; }
        protected IDisposable SignalrHost { get; set; }

        private List<GameServer> _servers;

        public void Start(string pathToWebFolder)
        {
            Port = FreeTcpPort();
            Path = String.Format("http://localhost:{0}/", Port);
            WebFolderPath = pathToWebFolder;
            _servers = new List<GameServer>();
            WebHost = WebApp.Start(Path, x =>
            {
                x.Use<Middleware>(x);
                x.Map("/signalr", map =>
                {
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration();
                    hubConfiguration.Resolver.Register(typeof(MainHub), () => new MainHub(this));
                    map.RunSignalR(hubConfiguration);
                }).UseNancy(op =>
                {
                    op.Bootstrapper = new Bootstrapper(this);
                });
                GlobalHost.HubPipeline.AddModule(new Modules.CallerCulturePipelineModule());
            });
        }

        static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public void Dispose()
        {
            lock (_servers)
            {
                foreach (var s in _servers)
                {
                    s.Dispose();
                }
            }
            WebHost.Dispose();
            SignalrHost.Dispose();
        }

        public void PingClients()
        {
            MainHub.Instance.Clients.All.Ping();
        }

        internal string HostGame(string username)
        {
            lock (_servers)
            {
                var ge = new GameEngine();
                var port = FreeTcpPort();
                var gs = new GameServer(port, ge);
                _servers.Add(gs);
                return port.ToString();
            }
        }

        private class Middleware : OwinMiddleware
        {
            private readonly ILogger _logger;

            public Middleware(OwinMiddleware next, IAppBuilder app)
                : base(next)
            {
                _logger = app.CreateLogger<Middleware>();
            }

            public async override Task Invoke(IOwinContext context)
            {
                _logger.WriteVerbose(
                    string.Format("{0} {1}: {2}",
                    context.Request.Scheme,
                    context.Request.Method,
                    context.Request.Path));

                await Next.Invoke(context);
            }
        }
    }
}