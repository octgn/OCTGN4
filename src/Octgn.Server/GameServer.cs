﻿using Octgn.Shared;
using Octgn.Shared.Models;
using Octgn.Shared.Networking;
using System;

namespace Octgn.Server
{
    public class GameServer : IGameServer, IDisposable
    {
        public int Id { get; private set; }
        public int Port { get; private set; }
        public string Name { get; private set; }
        internal GameEngine Engine { get; private set; }

        private static int _nextId = 0;
        private GameServerListener _listener;

        public GameServer(string name, GameResourceProvider resources)
        {
            Id = System.Threading.Interlocked.Increment(ref _nextId);
            Engine = new GameEngine(resources);
            Port = NetworkHelper.FreeTcpPort();
            Name = name;
            _listener = new GameServerListener(Port, new GameServerModel(Id, Name, Port), OnSocket);
        }

        private void OnSocket(GameSocket sock)
        {
            var user = new UnauthenticatedUser(this, sock);
            Engine.Users.AddUser(user);
        }

        public void Dispose()
        {
            _listener.Dispose();
            Engine.Dispose();
        }
    }
}