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
using System.Threading.Tasks;
using Akkatecture.Examples.Api.Domain.Repositories.Operations;
using Microsoft.AspNetCore.Mvc;

namespace Akkatecture.Examples.Api.Controllers
{
    public class OperationsController : BaseController
    {
        private readonly IQueryOperations _operationQuery;
        
        public OperationsController(
            IQueryOperations operationQuery)
        {
            _operationQuery = operationQuery; 
        }
        
        [HttpGet("operations")]
        public async Task<IActionResult> GetResources()
        {
            var operations = await _operationQuery.FindAll();

            return Ok(operations);
        }
        
        [HttpGet("operations/{id:Guid}")]
        public async Task<IActionResult> GetResources([FromRoute]Guid id)
        {
            var operation = await _operationQuery.Find(id);

            if (operation == null)
                return NotFound();

            if (operation.Percentage == 100)
            {
                var uri = new Uri($"{GetAbsoluteUri(HttpContext)}/api/resources/{id}");
                return SeeOther(uri);
            }

            return Ok(operation);
        }
    }
}