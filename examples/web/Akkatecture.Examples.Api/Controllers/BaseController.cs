// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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