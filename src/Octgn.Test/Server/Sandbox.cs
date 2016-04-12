using NUnit.Framework;
using Octgn.Server;
using Octgn.Server.JS;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void Spec()
        {
            using (var engine = new GameEngine(null))
            {
                var o = new OClass(engine);
                engine.Javascript.Execute("O.state.test = {}");
                engine.Javascript.Execute("O.state.test.jimmy = 12;");
                var val = engine.Javascript.Script.O.state.test.jimmy;
                //    var list = engine.StateHistory.GetLatestChanges()
                //        .Where(x => x.Name.StartsWith("O.state.test"))
                //        .ToArray();

                //    Assert.AreEqual(12, val);
                //    Assert.AreEqual("O.state.test", list[0].Name);
                //    Assert.AreEqual("{}", list[0].Change);
                //    Assert.AreEqual("O.state.test.jimmy", list[1].Name);
                //    Assert.AreEqual("12", list[1].Change);
            }
        }

        [Test]
        public void StateArraySpec()
        {
			using(var engine = new GameEngine(null))
			{
				var results = new Dictionary<string,bool>();
				engine.Javascript.AddObject("results", results);

				engine.Javascript.Execute("O.state.test = [12,13]");
                engine.Javascript.Execute(@"
var items = O.state.test.splice(0, 1);
var ify = JSON.stringify(items);
");
                Assert.AreEqual("[12]", engine.Javascript.Script.ify);
                engine.Javascript.Execute("O.state.test.length = 0;");

				engine.Javascript.Execute(@"
results.Add('Count of 0', O.state.test.length === 0);
O.state.test.push(1);
results.Add('Count of 1', O.state.test.length === 1);
O.state.test.pop();
results.Add('Count of 0 after pop', O.state.test.length === 0);
");

				foreach (var r in results.Keys)
					Assert.IsTrue(results[r],r + " == false");

				engine.Javascript.Execute("O.state.test[0] = 12");
				Assert.AreEqual(12, engine.Javascript.Script.O.state.test[0]);
			}
        }
    }
}
