using NUnit.Framework;
using Octgn.Server;
using Octgn.Server.JS;
using System;

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
                engine.Javascript.Execute("var test = O.statefull({})");
                engine.Javascript.Execute("O.state.test = test");
                engine.Javascript.Execute("O.state.test.jimmy = 12;");
                var val = engine.Javascript.Script.test.jimmy;

                var str = Newtonsoft.Json.JsonConvert.SerializeObject(engine.Javascript.Script.test);
                Console.WriteLine(str);

                Assert.AreEqual(12, val);
                //engine.Javascript.Execute("O.statefull({})");
                //engine.Javascript.Execute("O.state.users.push(1)");
                //engine.Javascript.Execute("O.state.jim = 1");
                //var d = engine.Javascript.ExecuteAndReturn("O.state.users.constructor.name");
                //System.Console.WriteLine(d);
            }
        }
    }
}
