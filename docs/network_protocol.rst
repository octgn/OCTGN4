++++++++++++++++
Network Protocol
++++++++++++++++

It should be noted that the endian order of the protocol is defined by the very first byte that gets sent. 0xFF if it's BigEndian and 0xFE if it's LittleEndian. Both client and server are required to send this initial byte immediately after connecting.

Packet Types
=================

Each Packet Type starts with a 1 Byte header declaring its type. That value is next to each header below(ex. Byte - 0x02). Each Packet Type also  ends with the byte 0x01. 

Byte - 0x02
-------------
* [1 Byte]

Short - 0x03
-------------
* [2 Byte]

Int32 - 0x04
-------------
* [4 Bytes]

Int64 - 0x05
-------------
* [8 Bytes]

Float - 0x06
-------------
* [4 Bytes]

String - 0x07
-------------

* [1 Byte] Length of String Length
* [1-20 Bytes] Length of String in ASCII
* [Bytes] UTF8 String

JSON Serialized Object - 0x08
-----------------------------
* [1 String] Object serialized to JSON

Method Parameter - 0x0A
------------------------

* [1 String] Name of the parameter
* [1 Packet Variable(Excluding Method Parameter)] Value of the parameter
   
Packet - 0xC8
-------------

* [1 String] Method Name
* [1 Byte] Number of Method Parameters
* [Parameters] Method Parameters
* [1 Byte] Packet End Delimiter - 0x01
