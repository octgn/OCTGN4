﻿using Nancy;
using Nancy.ModelBinding;
using Octgn.UI.Models.Home;

namespace Octgn.UI.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(UserSessions sessions)
        {
            Get["/"] = x =>
            {
                return View["Index"];
            };

            Get["/Login"] = _ => View["Login", new LoginModel()];

            Post["/Login"] = data =>
            {
                var login = this.BindAndValidate<LoginModel>();
                if (!this.ModelValidationResult.IsValid)
                {
                    return Negotiate
                        .WithModel(this.ModelValidationResult.FormattedErrors)
                        .WithStatusCode(HttpStatusCode.BadRequest);
                }

                var user = sessions.Create(login.Username);
                return Response.AsRedirect("/?sid=" + user.Sid);
            };

        }
    }
}


