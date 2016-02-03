using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Octgn.UI.Extensions
{
    public class ControllerActivator : DefaultControllerActivator
    {
        public ControllerActivator(ITypeActivatorCache typeActivatorCache)
            : base(typeActivatorCache)
        {

        }

        public override object Create(ActionContext actionContext, Type controllerType)
        {
            var cont = base.Create(actionContext, controllerType) as Controller;
            if (actionContext.HttpContext.User == null) return cont;
            if ((actionContext.HttpContext.User.Identity is User) == false) return cont;
            cont.ViewBag.Sid = (actionContext.HttpContext.User.Identity as User).Sid;
            return cont;
        }
    }
}
