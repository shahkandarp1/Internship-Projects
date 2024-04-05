using HalloDoc.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Auth
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        private readonly string _page;

        public CustomAuthorize(string role = "", string page = null)
        {
            _role = role;
            _page = page;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();

            if (jwtService == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "PatientLogin" }));
                return;
            }

            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "PatientLogin" }));
                return;
            }

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var menu = jwtToken.Claims.FirstOrDefault(c => c.Type == "Menus").Value;

            if (role == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "PatientLogin" }));
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || !_role.Contains(role.Value) )
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                return;
            }
            
            if(_page!=null)
            {
                if(!menu.Contains(_page))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                    return;
                }
            }
        }

    }
}
