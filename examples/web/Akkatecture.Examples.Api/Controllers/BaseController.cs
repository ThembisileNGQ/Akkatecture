using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Akkatecture.Examples.Api.Controllers
{
    public class BaseController : Controller
    {
        public static string GetAbsoluteUri(HttpContext context)
        {
            var request = context.Request;

            var host = request.Host.ToUriComponent();
            var scheme = request.Scheme;

            if (!(host.Contains("localhost") || host.Contains("127.0.0.1")))
            {
                scheme = "https";
            }

            return String.Concat(
                scheme,
                "://",
                request.Host.ToUriComponent());
        }
        
        [NonAction]
        protected IActionResult SeeOther(Uri location)
        {
            HttpContext.Response.GetTypedHeaders().Location = location;
            return StatusCode(StatusCodes.Status303SeeOther);
        }
    }
}