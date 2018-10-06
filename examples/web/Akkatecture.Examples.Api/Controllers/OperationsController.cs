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