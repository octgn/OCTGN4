using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Octgn.Server;
using Microsoft.ClearScript.V8;

namespace Octgn.Test
{
    [TestFixture]
    public class Sandbox
    {
        [Test]
        public void Run()
        {
            using (var engine = new V8ScriptEngine())
            {
                var so = new StateObject();
                dynamic d = so;
                engine.AddHostObject("O", so);

                engine.Execute(@"
O.taco = 12;
O.taco2 = {
    bean: 12
};
O.taco3 = {
    man: 100,
    _meta: {
        filter: 'jeans'
    }
}
");
                Assert.AreEqual(100, d.taco3.man);

                engine.Execute("O.taco3.man = 99;");

                Assert.AreEqual(99, d.taco3.man);

                d.taco3.man = 2;

                Assert.AreEqual(2, engine.Script.O.taco3.man);
            }
        }

        [Test]
        public void Whatever()
        {
            var rip = new GameResourceProvider(@"C:\Users\itsth\Programming\OCTGN4\src\Octgn.UI\Games\Test");
            using (var engine = new GameEngine(rip))
            {

            }
        }
    }
}
