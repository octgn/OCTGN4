﻿using NUnit.Framework;
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
                engine.Javascript.Execute("var test = statefull({})");
                engine.Javascript.Execute("O.state.test = test");
                engine.Javascript.Execute("O.state.test.jimmy = 12;");
                var val = engine.Javascript.Script.test.jimmy;
				var list = engine.StateHistory.GetLatestChanges()
					.Where(x=>x.Name.StartsWith("O.state.test"))
					.ToArray();

                Assert.AreEqual(12, val);
				Assert.AreEqual("O.state.test", list[0].Name);
				Assert.AreEqual("{}", list[0].Change);
				Assert.AreEqual("O.state.test.jimmy", list[1].Name);
				Assert.AreEqual("12", list[1].Change);
			}

			using(var engine = new GameEngine(null))
			{
				var results = new Dictionary<string,bool>();
				engine.Javascript.AddObject("results", results);

				engine.Javascript.Execute("O.state.test = statefull([])");

				engine.Javascript.Execute(@"
results.Add('Count of 0', O.state.test.array.length === 0);
O.state.test.push(1);
results.Add('Count of 1', O.state.test.array.length === 1);
O.state.test.pop();
results.Add('Count of 0 after pop', O.state.test.array.length === 0);
");

				var changes = engine.StateHistory.GetLatestChanges().ToArray();
				foreach (var r in results.Keys)
					Assert.IsTrue(results[r],r + " == false");

				//engine.Javascript.Execute("O.state.test.push(1)");

				//Assert.AreEqual(1, engine.Javascript.Script.test[0]);
			}
        }
    }
}
