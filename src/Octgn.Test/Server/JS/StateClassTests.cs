using NUnit.Framework;
using Octgn.Server;
using Octgn.Server.JS;
using System;
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
            GameThread.IgnoreThreadRestrictions = true;
            using (var ge = new GameEngine(null))
            {
                ge.EngineInitialized.WaitOne();
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

        [Test]
        public void AddUser()
        {
            GameThread.IgnoreThreadRestrictions = true;
            var id = 12;
            var username = "jim";
            
            using(var ge = new GameEngine(null))
            {
                ge.EngineInitialized.WaitOne(5000);
                var obj = new StateClass(ge);
                var usr = obj.AddUser(id, username);
                Assert.AreEqual(id, usr.id);
                Assert.AreEqual(username, usr.username);

                Assert.NotNull(ge.Javascript.Script.O.state.users[id]);
                Assert.AreEqual(username, ge.Javascript.Script.O.state.users[id].username);
                Assert.AreEqual(id, ge.Javascript.Script.O.state.users[id].id);
            }
        }
    }
}
