using NUnit.Framework;
using Octgn.Server;
using Octgn.Server.JS;

namespace Octgn.Test.Server
{
    [TestFixture]
    public class Sandbox
    {
        [Test]
        public void Whatever()
        {
            using(var engine = new GameEngine(null))
            {
                var o = new OClass(engine);
                engine.Javascript.
            }
       }
    }
}
