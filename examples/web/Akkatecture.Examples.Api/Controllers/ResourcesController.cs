using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Akka;
using Akkatecture.Examples.Api.Controllers.Models;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Commands;
using Akkatecture.Examples.Api.Domain.Repositories.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Akkatecture.Examples.Api.Controllers
{
    public class ResourceController : BaseController
    {
        private readonly IActorRef<ResourceManager> _resourceManager;
        private readonly IQueryResources _resourceQuery;
        public ResourceController(
            IActorRef<ResourceManager> resourceManager,
            IQueryResources resourceQuery)
        {
            _resourceManager = resourceManager;
            _resourceQuery = resourceQuery;
        }

        [HttpPost("resources")]
        public async Task<IActionResult> PostResource()
        {
            var resourceId = ResourceId.New;
            var id = resourceId.GetGuid();
            var command = new CreateResourceCommand(resourceId);

            var result = await _resourceManager.Ask<IExecutionResult>(command);

            if (result.IsSuccess)
            {
                var location = new Uri($"{GetAbsoluteUri(HttpContext)}/api/operations/{id}");
                var contentLocation = new Uri($"{GetAbsoluteUri(HttpContext)}/api/resources/{id}");
                HttpContext.Response.Headers.Add("Location", location.ToString());
                HttpContext.Response.Headers.Add("Content-Location", contentLocation.ToString());
                
                return Accepted(new ResourceResponseModel(id,id));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("resources")]
        public async Task<IActionResult> GetResources()
        {
            var resources = await _resourceQuery.FindAll();

            return Ok(resources);
        }
        
        [HttpGet("resources/{id:Guid}")]
        public async Task<IActionResult> GetResources([FromRoute]Guid id)
        {
            var resources = await _resourceQuery.Find(id);

            return Ok(resources);
        }
        

    }
}