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

REIDX
-----

**Applies To**: StatefullArray

Reindexes part or all of an array.

* [Reference Number] Array Reference Id
* [Integer] Where to start reindexing(Inclusive)
* [Integer] Length of section to reindex
* [Array[Integer]] Array of indexes in their new order

.. code-block:: javascript
    :linenos:

    O.state.jim = O.statefull();
    //NEW	1	0	{}

    O.state.tim = 12
    //SET	0	"tim"	12

    O.state.tim = O.state.jim
    //SET	0	"tim" ^1

    delete O.state.tim
    //DEL	0	["tim"]

    O.state.jim = O.statefullArray();
    //NEW	1	0	[]
    // []

    O.state.jim.push(1);
    //ADD	1	-1	[1]
    // [1]

    O.state.jim[0] = 12;
    //SET	1	0	12
    // [12]

    O.state.jim[1] = O.state.jim;
    //SET	1	1	^1
    // [12, [12, [12 ...]]]

    O.state.jim.pop();
    //DEL	1	[1]
    // [12]

    O.state.jim.push(1);
    //ADD	1	-1	[1]
    //[12,1]
    O.state.jim.push(1);
    //ADD	1	-1	[1]
    //[12,1,1]

    O.state.jim.length = 1
    //DEL	1	[1,2]
    //[12]

    O.state.jim.push(1,2,3);
    //ADD   1   -1  [1,2,3]
    //[12,1,2,3]

    O.state.jim.splice(1,2,"asdf");
    //DEL   1   [1,2]
    //[12,3]
    //ADD   1   1   ["asdf"]
    //[12,"asdF",3]

    O.state.jim.reverse();
    // REIDX    1   0   3   [3,2,1]
    //[3,"asdf",12]
