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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Akka;

namespace Akkatecture.Examples.Api.Domain.Repositories.Resources
{
    public class ResourcesQueryHandler : IQueryResources
    {
        private readonly ActorRefProvider<ResourcesStorageHandler> _resourceStorageHandler;
        
        public ResourcesQueryHandler(
            ActorRefProvider<ResourcesStorageHandler> resourceStorageHandler)
        {
            _resourceStorageHandler = resourceStorageHandler;
        }
        public async Task<ResourcesReadModel> Find(Guid id)
        {
            var query = new GetResourcesQuery();
            
            var result = await _resourceStorageHandler.Ask<List<ResourcesReadModel>>(query);

            var readModel = result.SingleOrDefault(x => x.Id == id);
            
            return readModel;
        }

        public async Task<IReadOnlyCollection<ResourcesReadModel>> FindAll()
        {
            var query =new GetResourcesQuery();
            
            var result = await _resourceStorageHandler.Ask<List<ResourcesReadModel>>(query);

            return result;
        }
    }

    public class GetResourcesQuery
    {
        
    }
}