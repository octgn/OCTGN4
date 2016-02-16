using NUnit.Framework;
using Octgn.Server;
using Octgn.Server.JS;

namespace Octgn.Test.Server
{
    [TestFixture]
    public class Sandbox
    {
        [Test]
        public void Whatever()
        {
            using (var engine = new GameEngine(null))
            {
                var o = new OClass(engine);

                engine.Javascript.Execute("var a = new Array();");
                var obj = engine.Javascript.ExecuteAndReturn("a");
            }
        }

        [Test]
        public void PreserveWrappedObject()
        {
            using (var engine = new GameEngine(null))
            {
                var o = new OClass(engine);
                var b = engine.Javascript.ExecuteAndReturn("O.statefull([])");
                engine.Javascript.Execute("O.state.users.push(1)");
                engine.Javascript.Execute("O.state.jim = 1");
                var d = engine.Javascript.ExecuteAndReturn("O.state.users.constructor.name");
                System.Console.WriteLine(d);
            }
        }
    }
}
