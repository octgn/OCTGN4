using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Octgn.Server.Networking
{
    public class NetworkProtocol
    {
        // TODO Need to handle disconnects in read somehow
        private Stream _stream;
        private Socket _socket;

        public NetworkProtocol(TcpClient client)
        {
            _stream = client.GetStream();
            _socket = client.Client;
        }

        public byte ReadByte()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x02) throw new InvalidDataException($"Trying to ReadByte but streams identifier byte is {ibyte}");
            return (byte)_stream.ReadByte();
        }

        public short ReadShort()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x03) throw new InvalidDataException($"Trying to ReadShort but streams identifier byte is {ibyte}");
            var arr = new byte[2];
            _stream.Read(arr, 0, 2);
            return BitConverter.ToInt16(arr, 0);
        }

        public int ReadInt32()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x04) throw new InvalidDataException($"Trying to ReadInt32 but streams identifier byte is {ibyte}");
            var arr = new byte[4];
            _stream.Read(arr, 0, 4);
            return BitConverter.ToInt32(arr, 0);
        }

        public long ReadInt64()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x05) throw new InvalidDataException($"Trying to ReadInt64 but streams identifier byte is {ibyte}");
            var arr = new byte[8];
            _stream.Read(arr, 0, 8);
            return BitConverter.ToInt64(arr, 0);
        }

        public float ReadFloat()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x06) throw new InvalidDataException($"Trying to ReadFloat but streams identifier byte is {ibyte}");
            var arr = new byte[4];
            _stream.Read(arr, 0, 4);
            return BitConverter.ToSingle(arr, 0);
        }

        public string ReadString()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x07) throw new InvalidDataException($"Trying to ReadString but streams identifier byte is {ibyte}");
            var lenlen = (int)ReadByte();
            var lenbytes = new byte[lenlen];
            _stream.Read(lenbytes, 0, lenlen);
            var lenstr = System.Text.Encoding.ASCII.GetString(lenbytes);
            var len = Int64.Parse(lenstr);

            var arrText = new byte[4096];
            var sb = new StringBuilder();
            for (long i = 0; i < len; )
            {
                var takeCount = (int)Math.Min(4096, len - i);
                _stream.Read(arrText, 0, takeCount);
                var str = Encoding.UTF8.GetString(arrText, 0, takeCount);
                sb.Append(str);

                i += takeCount;
            }
            return sb.ToString();
        }

        public object ReadVariable()
        {
            var ibytes = new byte[1];
            _socket.Receive(ibytes, 0, 1, SocketFlags.Peek);
            var ibyte = ibytes[0];
            switch (ibyte)
            {
                case 0x02:
                    return ReadByte();
                case 0x03:
                    return ReadShort();
                case 0x04:
                    return ReadInt32();
                case 0x05:
                    return ReadInt64();
                case 0x06:
                    return ReadFloat();
                case 0x07:
                    return ReadString();
                default:
                    throw new InvalidDataException($"Tried to ReadVariable. Got byte {ibyte}");
            }
        }

        public MethodParameter ReadMethodParameter()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0x08) throw new InvalidDataException($"Trying to ReadMethodParameter but streams identifier byte is {ibyte}");
            var name = ReadString();
            var val = ReadVariable();
            var ret = new MethodParameter
            {
                Name = name,
                Value = val
            };
            return ret;
        }

        public Packet ReadPacket()
        {
            var ibyte = (byte)_stream.ReadByte();
            if ((ibyte) != 0xC8) throw new InvalidDataException($"Trying to ReadPacket but streams identifier byte is {ibyte}");
            var mname = ReadString();
            var pnum = (int)ReadByte();
            var parr = new MethodParameter[pnum];
            for(var i = 0;i< pnum; i++)
            {
                parr[i] = ReadMethodParameter();
            }

            var ret = new Packet
            {
                Name = mname,
                Parameters = parr
            };
            return ret;
        }

        public void WriteByte(byte b)
        {
            _stream.WriteByte(0x02);
            _stream.WriteByte(b);
        }

        public void WriteShort(short s)
        {
            _stream.WriteByte(0x03);
            var bytes = BitConverter.GetBytes(s);
            _stream.Write(bytes, 0, bytes.Length);
            for(var i = 0; i < (2 - bytes.Length); i++)
            {
                _stream.WriteByte(0);
            }
        }

        public void WriteInt32(Int32 s)
        {
            _stream.WriteByte(0x04);
            var bytes = BitConverter.GetBytes(s);
            _stream.Write(bytes, 0, bytes.Length);
            for(var i = 0; i < (4 - bytes.Length); i++)
            {
                _stream.WriteByte(0);
            }
        }

        public void WriteInt64(Int64 s)
        {
            _stream.WriteByte(0x05);
            var bytes = BitConverter.GetBytes(s);
            _stream.Write(bytes, 0, bytes.Length);
            for(var i = 0; i < (8 - bytes.Length); i++)
            {
                _stream.WriteByte(0);
            }
        }

        public void WriteFloat(float s)
        {
            _stream.WriteByte(0x06);
            var bytes = BitConverter.GetBytes(s);
            _stream.Write(bytes, 0, bytes.Length);
            for(var i = 0; i < (4 - bytes.Length); i++)
            {
                _stream.WriteByte(0);
            }
        }

        public void WriteString(string s)
        {
            _stream.WriteByte(0x07);
            var lenbytes = Encoding.ASCII.GetBytes(s.Length.ToString());
            WriteByte((byte)lenbytes.Length);
            _stream.Write(lenbytes, 0, lenbytes.Length);
            var strBytes = Encoding.UTF8.GetBytes(s);
            _stream.Write(strBytes, 0, strBytes.Length);
        }
        public void WriteVariable(object o)
        {
            if(o is byte)
            {
                WriteByte((byte)o);
            }
            else if(o is short)
            {
                WriteShort((short)o);
            }
            else if(o is Int32)
            {
                WriteInt32((Int32)o);
            }
            else if(o is Int64)
            {
                WriteInt64((Int64)o);
            }
            else if(o is float)
            {
                WriteFloat((float)o);
            }
            else if(o is string)
            {
                WriteString((string)o);
            }
            else
            { 
                throw new InvalidDataException($"Tried to WriteVariable. Type {o.GetType()} is not valid.");
            }
        }

        public void WriteMethodParameter(MethodParameter p)
        {
            _stream.WriteByte(0x08);
            WriteString(p.Name);
            WriteVariable(p.Value);
        }

        public void WritePacket(Packet p)
        {
            _stream.WriteByte(0xC8);
            WriteString(p.Name);
            WriteByte((byte)p.Parameters.Length);
            for(var i = 0; i < p.Parameters.Length; i++)
            {
                WriteMethodParameter(p.Parameters[i]);
            }
        }

        public struct MethodParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                return GetHashCode() == obj.GetHashCode();
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }

        public class Packet
        {
            public string Name { get; set; }
            public MethodParameter[] Parameters { get; set; }

            public void Invoke<T>(T obj)
            {
                var methods = typeof(T).GetMethods();
                var method = methods.First(x => x.Name == Name);
                var parameters = method.GetParameters();
                var parr = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var mp = Parameters.FirstOrDefault(x => x.Equals(parameters[i].Name));
                    if (mp.Equals(default(MethodParameter)))
                    {
                        parr[i] = Activator.CreateInstance(parameters[i].ParameterType);
                    }
                    else
                    {
                        parr[i] = mp.Value;
                    }
                }

                method.Invoke(obj, parr);
            }
        }
    }
}
