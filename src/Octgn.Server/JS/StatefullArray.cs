using System.Dynamic;

namespace Octgn.Server.JS
{
    public class StatefullArray : StatefullObject
    {
        public StatefullArray(string name, GameEngine engine, DynamicObject obj)
        : base(name, engine, obj)
        {
			if (obj == null)
			{
				obj = (DynamicObject)engine.Javascript.ExecuteAndReturn("new Array();");
				this.dynamicObject = obj;
			}
        }

		public int push(params object[] args)
		{
			foreach(var o in args)
			{
				this.dynamicObject.array.push(o);
			}
			return args.Length;
		}

		public object pop()
		{
			return this.dynamicObject.array.pop();
		}
    }
}