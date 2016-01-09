﻿using System;
using NUnit.Framework;
using Octgn.Server.Networking;
using FakeItEasy;
using System.Collections.Generic;

namespace Octgn.Test.Server.Networking
{
    [TestFixture]
    public class PacketTests
    {
        [Test]
        public void Invoke_NoParameters([Values(true, false)]bool withArgs)
        {
            var packet = new NetworkProtocol.Packet()
            {
                Name = "Method1",
                Parameters = new NetworkProtocol.MethodParameter[0]
            };

            if (withArgs)
            {
                packet.Parameters = new NetworkProtocol.MethodParameter[]
                {
                    new NetworkProtocol.MethodParameter() {
                        Name = "a",
                        Value = "Asdf"
                    },
                };
            }

            var faker = A.Fake<ITest1>();
            packet.Invoke<ITest1>(faker);
            A.CallTo(() => faker.Method1()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test, Combinatorial]
        public void Invoke_MultipleParameters([Values(null, "asdf")]string arg1, [Values(null, 2)]int? arg2, [Values(null, 1.4f)]float? arg3)
        {
            var packet = new NetworkProtocol.Packet()
            {
                Name = "Method2",
            };

            var pars = new List<NetworkProtocol.MethodParameter>();
            if (arg1 != null) {
                var p = new NetworkProtocol.MethodParameter()
                {
                    Name = "a",
                    Value = arg1
                };
                pars.Add(p);
            }
            if (arg2 != null) {
                var p = new NetworkProtocol.MethodParameter()
                {
                    Name = "b",
                    Value = arg2.Value
                };
                pars.Add(p);
            }
            if (arg3 != null) {
                var p = new NetworkProtocol.MethodParameter()
                {
                    Name = "c",
                    Value = arg3.Value
                };
                pars.Add(p);
            }
            packet.Parameters = pars.ToArray();

            var faker = A.Fake<ITest1>();
            packet.Invoke<ITest1>(faker);
            var expectedA = arg1;
            var expectedB = arg2 == null ? default(int) : arg2.Value;
            var expectedC = arg3 == null ? default(float) : arg3.Value;
            A.CallTo(() => faker.Method2(A<string>.That.IsEqualTo(expectedA)
                , A<int>.That.IsEqualTo(expectedB)
                , A<float>.That.IsEqualTo(expectedC))
            ).MustHaveHappened(Repeated.Exactly.Once);
        }

        public interface ITest1
        {
            void Method1();
            void Method2(string a, int b, float c);
        }
    }
}
