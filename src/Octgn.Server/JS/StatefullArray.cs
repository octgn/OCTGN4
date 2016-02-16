using System.Dynamic;

namespace Octgn.Server.JS
{
    public class StatefullArray : StatefullObject
    {
        public StatefullArray(string name, GameEngine engine, DynamicObject obj)
        : base(name, engine, obj)
        {
        }
    }
}