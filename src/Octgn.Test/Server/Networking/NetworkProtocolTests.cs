using NUnit.Framework;
using Octgn.Server.Networking;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Octgn.Test.Server.Networking
{
    [TestFixture]
    public class NetworkProtocolTests
    {
        private TcpListener _listener;
        private TcpClient _socket1;
        private TcpClient _socket2;
        private NetworkProtocol _socket1Prot;
        private NetworkProtocol _socket2Prot;

        [OneTimeSetUp]
        public void Setup()
        {
            _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            var client = new TcpClient();
            client.ConnectAsync(IPAddress.Loopback, port);
            _socket2 = _listener.AcceptTcpClient();
            _socket1 = client;
            _listener.Stop();
            _socket1Prot = new NetworkProtocol(_socket1);
            _socket2Prot = new NetworkProtocol(_socket2);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _socket1.Dispose();
            _socket2.Dispose();
            _listener.Stop();
        }

        [Test]
        public void TestByte()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                _socket1Prot.WriteByte(i);
                var ret = _socket2Prot.ReadByte();
                Assert.AreEqual(i, ret);
            }
        }

        [Test]
        public void TestShort()
        {
            for (short i = short.MinValue; i < short.MaxValue; i++)
            {
                _socket1Prot.WriteShort(i);
                var ret = _socket2Prot.ReadShort();
                Assert.AreEqual(i, ret);
            }
        }

        [Test]
        public void TestInt32()
        {
            _socket1Prot.WriteInt32(Int32.MinValue);
            var ret = _socket2Prot.ReadInt32();
            Assert.AreEqual(Int32.MinValue, ret);

            _socket1Prot.WriteInt32(Int32.MaxValue);
            ret = _socket2Prot.ReadInt32();
            Assert.AreEqual(Int32.MaxValue, ret);

            _socket1Prot.WriteInt32(0);
            ret = _socket2Prot.ReadInt32();
            Assert.AreEqual(0, ret);
        }

        [Test]
        public void TestInt64()
        {
            _socket1Prot.WriteInt64(Int64.MinValue);
            var ret = _socket2Prot.ReadInt64();
            Assert.AreEqual(Int64.MinValue, ret);

            _socket1Prot.WriteInt64(Int64.MaxValue);
            ret = _socket2Prot.ReadInt64();
            Assert.AreEqual(Int64.MaxValue, ret);

            _socket1Prot.WriteInt64(0);
            ret = _socket2Prot.ReadInt64();
            Assert.AreEqual(0, ret);
        }

        [Test]
        public void TestFloat()
        {
            _socket1Prot.WriteFloat(float.MinValue);
            var ret = _socket2Prot.ReadFloat();
            Assert.AreEqual(float.MinValue, ret);

            _socket1Prot.WriteFloat(float.MaxValue);
            ret = _socket2Prot.ReadFloat();
            Assert.AreEqual(float.MaxValue, ret);

            _socket1Prot.WriteFloat(0);
            ret = _socket2Prot.ReadFloat();
            Assert.AreEqual(0, ret);
        }

        [Test]
        public void TestString()
        {
            var testList = new string[] {
                "",
                "    ",
                "\t   \r\n \r\r\t\n\t\0",
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLNOPQRSTUVWXYZ",
                "1234567890",
                "{}[]:;\"',.<>/?\\|`~!@#$%^&*()_+-="
            };

            foreach (var test in testList)
            {
                _socket1Prot.WriteString(test);
                var ret = _socket2Prot.ReadString();
                Assert.AreEqual(test, ret);
            }
        }

        [Test]
        public void TestVariables()
        {
            var testList = new object[] {
                byte.MaxValue,
                short.MaxValue,
                Int32.MaxValue,
                Int64.MaxValue,
                float.MaxValue,
                "\t   \r\n \r\r\t\n\t\0",
            };

            foreach (var test in testList)
            {
                _socket1Prot.WriteVariable(test);
                var ret = _socket2Prot.ReadVariable();
                Assert.AreEqual(test, ret);
            }

            // Make sure that generic object fails
            Assert.Throws<InvalidDataException>(() => _socket1Prot.WriteVariable(new object()));

            _socket1.GetStream().WriteByte(0xFF);
            Assert.Throws<InvalidDataException>(() =>_socket2Prot.ReadVariable());
            var bt = _socket2.GetStream().ReadByte();
            Assert.AreEqual(0xFF, bt);
        }

        [Test]
        public void TestMethodParameter()
        {
            var testList = new NetworkProtocol.MethodParameter[]
            {
                new NetworkProtocol.MethodParameter
                {
                    Name = "asdf",
                    Value = (float)1.323
                }
            };

            foreach (var test in testList)
            {
                _socket1Prot.WriteMethodParameter(test);
                var ret = _socket2Prot.ReadMethodParameter();
                Assert.AreEqual(test, ret);
            }
        }

        [Test]
        public void TestPacket()
        {
            var packet = new NetworkProtocol.Packet();
            packet.Name = "ChickenBoy";
            packet.Parameters = new NetworkProtocol.MethodParameter[] {
                new NetworkProtocol.MethodParameter() {
                    Name = "p1",
                    Value = "Asdf"
                },
                new NetworkProtocol.MethodParameter() {
                    Name = "p2",
                    Value = (byte)0xFF
                }
            };

            _socket1Prot.WritePacket(packet);
            var ret = _socket2Prot.ReadPacket();
            Assert.AreEqual(packet.Name, ret.Name);
            for(var i = 0; i < ret.Parameters.Length; i++)
            {
                Assert.AreEqual(packet.Parameters[i], ret.Parameters[i]);
            }
        }
    }
}
