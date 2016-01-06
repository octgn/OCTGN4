++++++++++++++++
OCTGN Workflow
++++++++++++++++

Issue Types
===========

Bug
---

A bug is a defect in the software. 

Open States
~~~~~~~~~~~

* No Label - This means that the issue was not reviewed. There is literally no label for this. 
* `Accepted <#it’s-a-bug-and-we-plan-on-fixing-it>`__ - This means that the issue passed initial inspection. 

Closed States 
~~~~~~~~~~~~~

* `No Repro <#can't-reproduce>`__ - Can't reproduce the bug 
* `More Info <#not-enough-info>`__ - This bug is lacking information before it can be fixed 
* `Expected <#the-bug-is-expected-behavior-aka-not-a-bug>`__ - This isn't a bug, it's actually the behavior we expect 
* `Won't Fix <#it’s-a-bug-but-we’re-not-going-to-fix-it>`__ - This is a bug, but we don't plan on fixing it. Must provide a reason why. 
* Fixed - The bug was fixed 
* Released - When this fix goes live. You must also include a version number as a comment #### Workflow

Give this the label ``Bug`` and continue

**🖐** Not Enough Info
~~~~~~~~~~~~~~~~~~~~~~

*Example* 
    I can't log in, help!

If the bug report doesn't have enough information, ``Close`` the bug and give it the label ``More Info``. Tell them why the bug report is closed.

This is generic text that can be used.

    This bug report does not contain enough information for us to act on so it has been closed. Please provide us more information and we will reevaluate this bug.

**🖐** Can't Reproduce
~~~~~~~~~~~~~~~~~~~~~~

Explain what you did to try and reproduce this bug. Then give it the label ``No Repro`` and ``close`` it.

Due to the nature of the applications and end users, there will be bugs that we won't be able to reproduce, but will need to be resolved reguardless. In those cases, give the issue the labels ``No Repro`` ``Accepted`` and leave it open. Once it can be reproduced, then we can remove ``No Repro`` and continue on with `fixing it <#it’s-a-bug-and-we-plan-on-fixing-it>`__

Post the following text as well

    This bug report was closed because we were unable to reproducde it.  If you provide more detailed information we will reevaluate this bug.

**🖐** The Bug is Expected Behavior *aka* Not a Bug
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

*Example*
    If I ``End Task`` the program, it doesn't save my changes.

Give this the label ``Expected`` and ``close`` the issue. Explain why this behavior is expected. Also paste in the following text

    This bug report has been closed because it is not actually a bug, it is expected behaviour. If you feel this is incorrect please provide us with more information and we will review it.

**🖐** It's a Bug, But We're Not Going To Fix It
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

*Example*
    While I'm using the application, if I click three times on the button, and then spin around in a circle, the application crashes

*(It's a bug, but it's so obscure in this case it's not worth spending the time on fixing it)*

Give this the label ``Won't Fix`` and ``close`` the issue. Explain why we won't fix this bug. Also paste in the following text

    This bug report has been closed because we don't plan on fixing this bug. Feel free to update us with more information if you feel it is relevant and we will review it, but at this time no further action will be taken.

**👍** It's a Bug, and We Plan on Fixing It
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

*Example*
    If I double click the button, it happens twice

Give this the label ``Accepted`` and assign it to someone to work on it.  If you don't know who should work on it, then don't change the label or assign it to the repo owner to delegate the task.

Add a line to the end of the description that shows the version it was introduced in(not the version it was discovered in) ex: ``Introduced In: 1.2.3.4 : username`` If the bug wasn't ever released live(it only existed in test builds or never even got released), put ``Introduced In: None : username`` . If you accept this bug, you *MUST* set this field.

Once the bug has been fixed, give it the label ``Fixed`` and close it.

Make sure this `issue is referenced <https://github.com/blog/957-introducing-issue-mentions>`__ in related pull requests and commits, and if your commit fixes the bug, you should `close the issue via a commit message <https://help.github.com/articles/closing-issues-via-commit-messages/>`__.

Once the fix is released to a specific version of the application, make sure to add a line to the end of the description that says ``Fixed In: 1.2.3.4 : username`` . If the bug wasn't fixed but is closed, make sure to change the labels to fit the current situation.

Feature Request
---------------

This is a request to add a feature to the software

Give this the label ``Bug`` and continue

Open States
~~~~~~~~~~~

-  No Label - This means that the issue was not reviewed.
-  Accepted - This means that the issue passed initial inspection. (Should detail what this means exactly, or what types of features should not be accepted)
-  Must Test - Means this feature has been implemented, but that needs to be tested before it can be closed 

Closed States
~~~~~~~~~~~~~

-  More Info - This feature is lacking information before it can be implemented
-  Won't Implement - We don't plan on implementing this feature. It must be explained in the comments of the issue as to why we won't.
-  Complete - The feature is tested and committed to the master branch.
-  Released - When this fix goes live. You must also include a version number as a comment

Workflow
~~~~~~~~

Give this the label ``Feature Request`` and continue

Issue Priority
============

Bugs and Feature Requests can have priority labels.

-  Blocking - High priority. If this is set **TODO fill the rest of this sentence**
-  {Username} - A special priority which allows {Username} to say that this issue is a priority item for them, even if it's not a priority otherwise. These labels must be the color #ABCDEF

Release Types
=============

There are 4 different types of releases 
1. Major Release 
2. Feature Release 
3. Bug Release 
4. Test Release

Version Schema
--------------

+ - Increment the number 
P - Use previous number 
0-9 - Explicit number

**Example**

::

    version = 1.2.3.4
    ApplySchema(version, "P.+.0.9")
    Assert.AreEqual(version, "1.3.0.9")

Major Release
-------------

**Version Schema**: +.0.0.0

This is unlikely to happen, and will most likely bring with it a whole new workflow. Basically this is a rewrite. 

Feature Release
---------------

**Version Schema**: P.+.0.0

If there are any `Blocking <#Blocking>`__ issues they must be either closed in this release or in a previous `Bug Release <#bug-release>`__

Bug Release
------------

**Version Schema**: P.P.+.0

This release type fixes issues that were introduced in any release type.

Test Release
------------

**Version Schema**: P.P.P.+

This release type ends up being the development builds that occure when we're doing `Bug Release <#bug-release>`__ or a `Feature Release <#feature-release>`__
