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
using Akkatecture.Akka;

namespace Akkatecture.Examples.Api.Domain.Repositories.Operations
{
    public class OperationsQueryHandler : IQueryOperations
    {
        private readonly ActorRefProvider<OperationsStorageHandler> _operationStorageHandler;
        public OperationsQueryHandler(
            ActorRefProvider<OperationsStorageHandler> operationStorageHandler)
        {
            _operationStorageHandler = operationStorageHandler;
        }
        public async Task<OperationsReadModel> Find(Guid operationId)
        {
            var query = new GetOperationsQuery();
            
            var result = await _operationStorageHandler.Ask<List<OperationsReadModel>>(query);

            var readModel = result.SingleOrDefault(x => x.Id == operationId);
            
            return readModel;
        }

        public async Task<IReadOnlyCollection<OperationsReadModel>> FindAll()
        {
            var query = new GetOperationsQuery();
            
            var result = await _operationStorageHandler.Ask<List<OperationsReadModel>>(query);

            var sortedResult = result
                .OrderBy(x => x.Percentage)
                .ThenBy(x => x.StartedAt)
                .ToList();
            
            return sortedResult;
        }
    }

    public class GetOperationsQuery
    {
        
    }
}