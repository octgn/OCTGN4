using System;
using System.IO;
using System.Linq;

namespace Octgn.Server
{
    public class GameResourceProvider
    {
        private DirectoryInfo _basePath;
        public GameResourceProvider(string basePath)
        {
            _basePath = new DirectoryInfo(basePath);
        }

        //TODO stuff to get script files and shit in the resources dir
        public string ReadEntryPoint()
        {
            var path = _basePath.GetFiles("startup.js")[0];
            return File.ReadAllText(path.FullName);
        }
    }
}
