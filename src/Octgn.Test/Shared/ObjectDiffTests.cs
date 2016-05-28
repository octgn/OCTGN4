using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Octgn.Shared;
using System.Dynamic;
using Octgn.Server.JS;
using Octgn.Server;
using Dynamitey;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Octgn.Test.Shared
{
    [TestFixture]
    public class ObjectDiffTests
    {
        [Test]
        public void ParameterlessConstructor()
        {
            Activator.CreateInstance<ObjectDiff>();
        }

        [Test, Combinatorial]
        public void IsValue_True([Values(null, "asdf", "", 12, 1.2)]object obj)
        {
            Assert.True(ObjectDiff.IsValue(obj));
        }

        [Test]
        public void IsValue_False()
        {
            Assert.False(ObjectDiff.IsValue(new object()));
        }

        [Test]
        public void EnumerateProperties()
        {
            GameThread.IgnoreThreadRestrictions = true;
            using (var ge = new GameEngine(null))
            {
                ge.EngineInitialized.WaitOne();
                var objs = new List<object>();

                var adders = new
                {
                    a = 12,
                    b = 14,
                    c = new
                    {
                        d = 15
                    }
                };
                var adderProps = adders.GetType().GetProperties();
                IDictionary<string, object> adderPropsDick = adderProps.ToDictionary(x => x.Name, x => x.GetValue(adders));
                {
                    dynamic obj = new ExpandoObject();
                    foreach (var prop in adderPropsDick)
                    {
                        (obj as IDictionary<string, object>).Add(prop.Key, prop.Value);
                    }
                    objs.Add(obj);
                }

                {
                    StateClass obj = new StateClass(ge);

                    foreach (var prop in adderPropsDick)
                    {
                        Dynamic.InvokeSet(obj, prop.Key, prop.Value);
                    }

                    objs.Add(obj);
                }

                {
                    objs.Add(adders);
                    objs.Add(adderPropsDick);
                    objs.Add(adderPropsDick.AsEnumerable());
                }

                foreach (var obj in objs)
                {
                    var objProps = ObjectDiff.EnumerateProperties(obj)
                        .Where(x => x.Name != "users")
                        .ToArray();
                    foreach (var prop in objProps)
                    {
                        Assert.Contains(prop.Name, adderPropsDick.Keys.ToArray());
                        Assert.AreEqual(prop.Value, adderPropsDick[prop.Name]);
                    }
                    foreach (var prop in adderPropsDick)
                    {
                        Assert.Contains(prop.Key, objProps.Select(x=>x.Name).ToArray());
                        Assert.AreEqual(prop.Value, objProps.First(x=>x.Name == prop.Key).Value);
                    }
                }
            }
        }

        [Test]
        public void EnumerateProperties_JObject()
        {
            GameThread.IgnoreThreadRestrictions = true;
            using (var ge = new GameEngine(null))
            {
                ge.EngineInitialized.WaitOne();
                var objs = new List<object>();

                var adders = new
                {
                    a = 12,
                    b = 14,
                    c = new
                    {
                        d = 15
                    }
                };

                var obj = JObject.FromObject(adders);

                var props = ObjectDiff.EnumerateProperties(obj).ToDictionary(x => x.Name, x => x.Value);

                Assert.AreEqual(12, ((JValue)props["a"]).Value);
                Assert.AreEqual(14, ((JValue)props["b"]).Value);
            }
        }

        [Test]
        public void Diff() {
            GameThread.IgnoreThreadRestrictions = true;
            using (var ge = new GameEngine(null)) {
                ge.EngineInitialized.WaitOne();
                var diff = new ObjectDiff();

                dynamic sobj = new ExpandoObject();
                dynamic eobj = new ExpandoObject();

                eobj.Tim = new ExpandoObject();
                eobj.Tim.bill = 12;

                diff.Diff(sobj, eobj);
                Assert.AreEqual(diff.Added["Tim"], ObjectDiff.ToJObject(eobj.Tim));
                Assert.IsEmpty(diff.Deleted);
                Assert.IsEmpty(diff.Modified);

                // Equal
                diff = new ObjectDiff();
                sobj = eobj;
                eobj = new ExpandoObject();

                eobj.Tim = new ExpandoObject();
                eobj.Tim.bill = 12;

                diff.Diff(sobj, eobj);
                Assert.IsEmpty(diff.Added);
                Assert.IsEmpty(diff.Deleted);
                Assert.IsEmpty(diff.Modified);

                // Modify
                diff = new ObjectDiff();
                sobj = eobj;
                eobj = new ExpandoObject();

                eobj.Tim = new ExpandoObject();
                eobj.Tim.bill = "chicken";

                diff.Diff(sobj, eobj);
                Assert.IsEmpty(diff.Added);
                Assert.IsEmpty(diff.Deleted);
                Assert.AreEqual(((JValue)diff.Modified["Tim.bill"]).Value, "chicken");

                // Delete
                diff = new ObjectDiff();
                sobj = eobj;
                eobj = new ExpandoObject();

                eobj.Tim = new ExpandoObject();

                diff.Diff(sobj, eobj);
                Assert.IsEmpty(diff.Added);
                Assert.IsEmpty(diff.Modified);
                Assert.Contains("Tim.bill", diff.Deleted);

                eobj = new ExpandoObject();

                diff.Diff(sobj, eobj);
                Assert.IsEmpty(diff.Added);
                Assert.IsEmpty(diff.Modified);
                Assert.Contains("Tim", diff.Deleted);
            }
        }

        [Test]
        public void Patch()
        {
            var po = ObjectDiff.ToJObject(new
            {
                a = 1,
                b = 2,
                c = new
                {
                    d = 3
                }
            });

            var co = ObjectDiff.ToJObject(new
            {
                a = 1,
                b = 2,
                c = new
                {
                    d = 3
                }
            });

            co["a"] = 2;
            (co["c"] as JObject).Add("e", 4);
            co.Remove("b");

            var diff = new ObjectDiff(po, co);

            var patch = diff.Patch(po) as JObject;

            Assert.AreEqual(((JValue)co["a"]).Value, ((JValue)patch["a"]).Value);
            Assert.IsNull(patch.Property("b"));
            Assert.AreEqual(4, ((JValue)((JObject)patch["c"])["e"]).Value);
        }

        [Test]
        public void IsArray()
        {
            var js = new JavascriptEngine();
            var arrays = new object[] {
                js.ExecuteAndReturn("[\"a\"]"),
                new string[1] {"a"},
            };
            var notArrays = new object[]
            {
                js.ExecuteAndReturn("\"a\""),
                new ExpandoObject(),
                new Dictionary<int, string>()
            };

            for (var i = 0; i < arrays.Length; i++)
            {
                try
                {
                    Assert.True(ObjectDiff.IsArray(arrays[i]), "Item {0} is an array", i);
                }
                catch (Exception e)
                {
                    throw new Exception($"Item {i} in arrays threw an exception", e);
                }
            }

            for (var i = 0; i < notArrays.Length; i++)
            {
                try
                {
                    Assert.False(ObjectDiff.IsArray(notArrays[i]), "Item {0} is not an array", i);
                }
                catch (Exception e)
                {
                    throw new Exception($"Item {i} in notArrays threw an exception", e);
                }
            }
        }

        [Test]
        public void DiffKeysWithArrays()
        {
            using(var engine = new JavascriptEngine()) {
                engine.Execute("var a = {};");
                var startObj = engine.ExecuteAndReturn("a");

                engine.Execute("var a = {};");
                engine.Execute("a.users = []");
                var endObj = engine.ExecuteAndReturn("a");

                // Added
                var diff = new ObjectDiff();
                diff.Diff(startObj, endObj, "a");

                Assert.AreEqual(1, diff.Added.Count);
                Assert.AreEqual("a.users", diff.Added.Keys.First(), "It actually equals " + diff.Added.Keys.First());

                var val = JsonConvert.SerializeObject(diff.Added.First().Value);
                Assert.AreEqual("[]", val);
            }
            using(var engine = new JavascriptEngine())
            {
                engine.Execute("var a = {};");
                engine.Execute("a.users = []");
                var startObj = engine.ExecuteAndReturn("a");

                engine.Execute("var a = {};");
                engine.Execute("a.users = []");
                engine.Execute("a.users[1] = 12");
                var endObj = engine.ExecuteAndReturn("a");

                // Added
                var diff = new ObjectDiff();
                diff.Diff(startObj, endObj, "a");

                Assert.AreEqual(2, diff.Added.Count);
                Assert.True(diff.Added.ContainsKey("a.users[0]"));
                Assert.True(diff.Added.ContainsKey("a.users[1]"));

                // Deleted
                diff = new ObjectDiff();
                diff.Diff(endObj, startObj, "a");                

                Assert.AreEqual(2, diff.Deleted.Count);
                Assert.True(diff.Deleted.Contains("a.users[0]"));
                Assert.True(diff.Deleted.Contains("a.users[1]"));

                // Modified
                engine.Execute("var a = {};");
                engine.Execute("a.users = []");
                engine.Execute("a.users[1] = 14");
                var changedObj = engine.ExecuteAndReturn("a");

                diff = new ObjectDiff();
                diff.Diff(endObj, changedObj, "a");                

                Assert.AreEqual(1, diff.Modified.Count);
                Assert.AreEqual("a.users[1]", diff.Modified.Keys.First(), "It actually equals " + diff.Modified.Keys.First());
            }
        }
    }
}
