﻿namespace Octgn.Server.Networking
{
    public interface IC2SComs
    {
        void Hello(string username);
    }

    public interface IS2CComs
    {
        void Kicked(string message);
    }
}