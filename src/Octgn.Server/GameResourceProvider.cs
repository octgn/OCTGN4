using Octgn.Shared.Models;
using System;
using System.IO;

namespace Octgn.Server
{
    public class GameResourceProvider
    {
		public GameManifest Manifest { get; set; }
        private DirectoryInfo _basePath;
        public GameResourceProvider(string basePath)
        {
            _basePath = new DirectoryInfo(basePath);
			var path = _basePath.GetFiles("manifest.json")[0];
            Manifest = GameManifest.Parse(File.ReadAllText(path.FullName));
        }

        //TODO stuff to get script files and shit in the resources dir
        public string ReadEntryPoint()
        {
            var path = _basePath.GetFiles("startup.js")[0];
            return File.ReadAllText(path.FullName);
        }

		public Resource Get(string path)
		{
			var p = Path.Combine(_basePath.FullName, "Resources", path);
			var ret = new Resource(p);
			ret.Read();
			return ret;
		}

		public class Resource
		{
			public string Path { get; private set; }
			public string ContentType { get; set; }
			public byte[] Data { get; set; }

			public bool Exists { get; private set; }

			public Resource(string path)
			{
				ContentType = "application/octet-stream";
				Data = new byte[0];
				Path = path;
				Exists = File.Exists(Path);
			}

			public void Read()
			{
				if (!Exists) return;
				ContentType = System.Web.MimeMapping.GetMimeMapping(Path);
				Data = File.ReadAllBytes(Path);
			}
		}
    }
}
