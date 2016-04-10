using NUnit.Framework;
using Octgn.Server;
using Octgn.Server.JS;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Octgn.Test.Server.JS
{
    [TestFixture]
    public class StateClassTests
    {
        [Test]
        public void ImplementsDynamicObject()
        {
            using (var ge = new GameEngine(null))
            {
                var obj = new StateClass(ge);
                Assert.IsInstanceOf<DynamicObject>(obj);

                dynamic dobj = obj;
                // Can add/modify properties dynamically
                {
                    dobj.Taco = 12;
                    Assert.AreEqual(12, dobj.Taco);

                    dobj.Taco = new
                    {
                        Bill = 14
                    };
                    Assert.AreEqual(14, dobj.Taco.Bill);

                    dobj.Taco = null;
                    Assert.IsNull(dobj.Taco);
                }
                // Test in javascript
                {
                    obj = new StateClass(ge);
                    dobj = obj;

                    ge.Javascript.AddObject("obj", obj);

                    ge.Javascript.Execute("obj.Taco = 12;");
                    Assert.AreEqual(12, dobj.Taco);

                    ge.Javascript.Execute("obj.Taco = {'Bill': 14};");
                    Assert.AreEqual(14, dobj.Taco.Bill);

                    ge.Javascript.Execute("delete obj.Taco");
                    Assert.False(obj.GetDynamicMemberNames().Contains("Taco"));
                }
            }
        }
    }
}
