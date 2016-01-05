++++++++++++++++
Network Protocol
++++++++++++++++

It should be noted that the endian order of the protocol is defined by the very first byte that gets sent. 0xFF if it's BigEndian and 0xFE if it's LittleEndian. Both client and server are required to send this initial byte immediately after connecting.

Packet Variables
=================

Byte
-----
* [1 Byte]

Short
-----
* [2 Byte]

Int32
------
* [4 Bytes]

Int64
------
* [8 Bytes]

Float
------
* [4 Bytes]

String
------

* [1 Byte] Length of String Length
* [1-20 Bytes] Length of String in ASCII
* [Bytes] UTF8 String

Method Parameter
----------------

* [1 String] Name of the parameter
* [1 Packet Variable(Excluding Method Parameter)] Value of the parameter
   
Packets
=======

Packet
-------

* [1 Byte] Packet Start Delimiter - 0x01
* [1 String] Method Name
* [Parameters] Method Parameters
* [1 Byte] Packet End Delimiter - 0x02
