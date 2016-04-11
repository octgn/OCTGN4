using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Octgn.Shared;
using System.Dynamic;
using Octgn.Server.JS;
using Octgn.Server;
using Dynamitey;

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
            using (var ge = new GameEngine(null))
            {
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
                    var objProps = ObjectDiff.EnumerateProperties(obj).ToDictionary(x=>x.Key, x=>x.Value);
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
        public void Diff()
        {
            using (var ge = new GameEngine(null))
            {
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
    }
}
