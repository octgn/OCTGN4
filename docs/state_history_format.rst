+++++++++++++++++++++
State History Format
+++++++++++++++++++++

Commands
=========


NEW
----

**Applies To**: StatefullObject, StatefullArray

* [Reference Number] New Reference Id
* [Reference Number] Parents Reference Id
* [Object,Array] Statefull object to add

SET
----

**Applies To**: StatefullObject, StatefullArray

* [Reference Number] Parents Reference Id
* [String, Integer] Property Name or Array Index
* [Value,Object,Reference Number] What to set the value to

UNSET
-----

**Applies To**: StatefullObject, StatefullArray

Makes the value undefined

* [Reference Number] Parents Reference Id
* [String, Integer] Property Name or Array Index

DEL
----

**Applies To**: StatefullObject, StatefullArray

Assumes all items following the delete are shifted back appropriatly

* [Reference Number] Parents Reference Id
* [Array[String, Integer]] Property Name or Array Index

ADD
----

**Applies To**: StatefullArray

* [Reference Number] Array Reference Id
* [Integer] Where to add to in the array
* [Array[Value,Object,Reference Number]] Things to add



.. code-block:: javascript
    :linenos:

    O.state.jim = O.statefull();
    //NEW	1	0	{}

    O.state.tim = 12
    //SET	0	tim	12

    O.state.tim = O.state.jim
    //SET	0	tim	*1

    delete O.state.tim
    //DEL	0	["tim"]

    O.state.jim = O.statefullArray();
    //NEW	1	0	[]

    O.state.jim.push(1);
    //ADD	1	-1	[1]

    O.state.jim[0] = 12;
    //SET	1	0	12

    O.state.jim[1] = O.state.jim;
    //SET	1	1	*1

    O.state.jim.pop();
    //DEL	1	[1]

    O.state.jim.push(1);
    //ADD	1	-1	[1]
    O.state.jim.push(1);
    //ADD	1	-1	[1]

    O.state.jim.length = 1
    //DEL	1	[1,2]
