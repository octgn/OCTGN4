﻿using System;
using Microsoft.AspNet.SignalR;
using Ninject;
using System.Collections.Generic;
using System.Linq;

namespace Octgn.UI
{
    internal class NinjectSignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;
        public NinjectSignalRDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
			if (_kernel.GetBindings(serviceType).Any())
				return _kernel.TryGet(serviceType);
			else
				return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}