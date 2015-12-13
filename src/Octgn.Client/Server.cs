﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Owin;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Windows;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Octgn.Client
{
    public class Server : IDisposable
    {
        //internal static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string WebFolderPath { get; private set; }
        public string Path { get; private set; }
        public int Port { get; private set; }
        public int SignalRPort { get; private set; }
        protected IDisposable WebHost { get; set; }
        protected IDisposable SignalrHost { get; set; }

        public void Start(string pathToWebFolder)
        {
            Port = FreeTcpPort();
            SignalRPort = FreeTcpPort();
            Path = String.Format("http://localhost:{0}/", Port);
            WebFolderPath = pathToWebFolder;
            WebHost = WebApp.Start(Path, x =>
            {
                x.Use<Middleware>(x);
                x.UseNancy(op =>
                {
                    op.Bootstrapper = new Bootstrapper(this);
                }).MaxConcurrentRequests(1);
            });
            SignalrHost = WebApp.Start(String.Format("http://localhost:{0}/", SignalRPort), x =>
            {
                x.Use<Middleware>(x);
                x.UseCors(CorsOptions.AllowAll);
                x.MapSignalR();
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
            WebHost.Dispose();
            SignalrHost.Dispose();
        }

        public void PingClients()
        {
            MainHub.Instance.Clients.All.Ping();
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

        private class SingleThreadedMiddleware : OwinMiddleware
        {
            private Thread _thread;
            private SynchronizationContext _context;
            private TaskScheduler _scheduler;
            public SingleThreadedMiddleware(OwinMiddleware next, IAppBuilder app)
                : base(next)
            {
                _thread = new Thread(Run);
            }

            private void Run()
            {
                _context = SynchronizationContext.Current;
                _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }

            public async override Task Invoke(IOwinContext context)
            {
                await Task.Factory.StartNew(() =>
                {
                    var t = Next.Invoke(context);
                    t.Wait();
                    return t;
                }, CancellationToken.None, TaskCreationOptions.None, _scheduler);
                //await Next.Invoke(context);
            }

        }
        private class SingleThreadedSynchronizationContext : SynchronizationContext
        {
            //TODO http://www.codeproject.com/Articles/32113/Understanding-SynchronizationContext-Part-II
            public override void Send(SendOrPostCallback d, object state)
            {
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                base.Post(d, state);
            }
        }
    }
}