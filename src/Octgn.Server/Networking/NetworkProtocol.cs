using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Octgn.Server.Networking
{
    public class NetworkProtocol
    {
        // TODO Need to handle disconnects in read somehow
        private Stream _stream;
        private Socket _socket;
        public NetworkProtocol(Socket socket)
        {
            _stream = new NetworkStream(socket);
            _socket = socket;
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
                var str = Encoding.UTF8.GetString(arrText);
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

        public struct MethodParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public class Packet
        {
            public string Name { get; set; }
            public MethodParameter[] Parameters { get; set; }
        }
    }
}
