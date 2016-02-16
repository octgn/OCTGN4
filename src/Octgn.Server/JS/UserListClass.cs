using System;

namespace Octgn.Server.JS
{
    public class UserListClass : StatefullArray
    {
        internal UserListClass(GameEngine engine, StateClass cls)
        : base("users", engine, null)
        {
        }

        internal void Add(UserClass user)
        {
        }
    }
}