using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Octgn.Shared;
using System.Dynamic;
using Octgn.Server.JS;
using Octgn.Server;
using Dynamitey;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

                //{
                //    var jstring = Newtonsoft.Json.JsonConvert.SerializeObject(adders);
                //    ge.Javascript.Execute("var test = " + jstring + ";");
                //    var obj = ge.Javascript.ExecuteAndReturn("test");

                //    objs.Add(obj);
                //}

                {
                    objs.Add(adders);
                    objs.Add(adderPropsDick);
                    objs.Add(adderPropsDick.AsEnumerable());
                }

                foreach (var obj in objs)
                {
                    var objProps = ObjectDiff.EnumerateProperties(obj)
                        .Where(x=>x.Key != "users")
                        .ToDictionary(x=>x.Key, x=>x.Value);
                    foreach (var prop in objProps)
                    {
                        Assert.Contains(prop.Key, adderPropsDick.Keys.ToArray());
                        Assert.AreEqual(prop.Value, adderPropsDick[prop.Key]);
                    }
                    foreach(var prop in adderPropsDick)
                    {
                        Assert.Contains(prop.Key, objProps.Keys.ToArray());
                        Assert.AreEqual(prop.Value, objProps[prop.Key]);
                    }
                }
            }
        }

        [Test]
        public void EnumerateProperties_JObject() {
            GameThread.IgnoreThreadRestrictions = true;
            using (var ge = new GameEngine(null)) {
                ge.EngineInitialized.WaitOne();
                var objs = new List<object>();

                var adders = new {
                    a = 12,
                    b = 14,
                    c = new {
                        d = 15
                    }
                };

                var obj = JObject.FromObject(adders);

                var props = ObjectDiff.EnumerateProperties(obj).ToDictionary(x => x.Key, x => x.Value);

                Assert.AreEqual(12, props["a"]);
                Assert.AreEqual(14, props["b"]);
            }
        }

        [Test]
        public void Diff()
        {
            GameThread.IgnoreThreadRestrictions = true;
            using (var ge = new GameEngine(null))
            {
                ge.EngineInitialized.WaitOne();
                var diff = new ObjectDiff();

                dynamic sobj = new ExpandoObject();
                dynamic eobj = new ExpandoObject();

                eobj.Tim = new ExpandoObject();
                eobj.Tim.bill = 12;

                diff.Diff(sobj, eobj);
                Assert.AreEqual(diff.Added["Tim"], eobj.Tim);
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
                Assert.AreEqual(diff.Modified["Tim.bill"], "chicken");

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
        public void ObjectToDictionary()
        {
            var po = ObjectDiff.ObjectToDictionary(new
            {
                a = 1,
                b = 2,
                c = new
                {
                    d = 3
                }
            });

            Assert.AreEqual(1, po["a"]);
            Assert.AreEqual(2, po["b"]);
            Assert.IsInstanceOf<Dictionary<string, object>>(po["c"]);
            Assert.AreEqual(3, ((Dictionary<string, object>)po["c"])["d"]);
        }

        [Test]
        public void ObjectToDictionary_ThrowsOnValues([Values(null, "asdf", "", 12, 1.2)]object obj)
        {
            Assert.Throws<InvalidOperationException>(()=>ObjectDiff.ObjectToDictionary(obj));
        }

        [Test]
        public void Patch()
        {
            var po = ObjectDiff.ObjectToDictionary(new {
                a = 1,
                b = 2,
                c = new {
                    d = 3
                }
            });

            var co = ObjectDiff.ObjectToDictionary(new
            {
                a = 1,
                b = 2,
                c = new
                {
                    d = 3
                }
            });

            co["a"] = 2;
            (co["c"] as Dictionary<string, object>).Add("e", 4);
            co.Remove("b");

            var diff = new ObjectDiff(po, co);

            var patch = diff.Patch(po) as Dictionary<string, object> ;

            Assert.AreEqual(co["a"], patch["a"]);
            Assert.False(patch.ContainsKey("b"));
            Assert.AreEqual(4, (patch["c"] as Dictionary<string, object>)["e"]);
        }

        [Test]
        public void IsArray()
        {
            var arrays = new object[] {
                new string[1] {"a"},

            };
            var notArrays = new object[]
            {

            };

            for(var i = 0;i<arrays.Length;i++)
            {
                try
                {
                    Assert.True(ObjectDiff.IsArray(arrays[i]), "Item {0} is an array", i);
                }
                catch(Exception e)
                {
                    throw new Exception($"Item {i} in arrays threw an exception", e);
                }
            }

            for(var i = 0;i< notArrays.Length;i++)
            {
                try
                {
                    Assert.False(ObjectDiff.IsArray(notArrays[i]), "Item {0} is not an array", i);
                }
                catch(Exception e)
                {
                    throw new Exception($"Item {i} in notArrays threw an exception", e);
                }
            }
        }
    }
}
