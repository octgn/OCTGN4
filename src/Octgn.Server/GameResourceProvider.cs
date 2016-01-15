﻿using Octgn.Shared.Models;
using System;
using System.IO;

namespace Octgn.Server
{
    public class GameResourceProvider
    {
		public Lazy<GameManifest> Manifest { get; set; }
        private DirectoryInfo _basePath;
        public GameResourceProvider(string basePath)
        {
            _basePath = new DirectoryInfo(basePath);
			Manifest = new Lazy<GameManifest>(() => {
				var path = _basePath.GetFiles("manifest.json")[0];
				return GameManifest.Parse(File.ReadAllText(path.FullName));
			});
        }

        //TODO stuff to get script files and shit in the resources dir
        public string ReadEntryPoint()
        {
            var path = _basePath.GetFiles("startup.js")[0];
            return File.ReadAllText(path.FullName);
        }
    }
}
