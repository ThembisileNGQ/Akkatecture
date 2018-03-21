using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Akkatecture.Extensions;

namespace Akkatecture.Core
{
    public static class ReflectionHelper
    {
        public static string GetCodeBase(Assembly assembly, bool includeFileName = false)
        {
            var codebase = assembly.CodeBase;
            var uri = new UriBuilder(codebase);
            var path = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
            var codeBase = includeFileName ?
                path :
                Path.GetDirectoryName(path);
            return codeBase;
        }

        /// <summary>
        /// Handles correct upcast. If no upcast was needed, then this could be exchanged to an <c>Expression.Call</c>
        /// and an <c>Expression.Lambda</c>.
        /// </summary>
        public static TResult CompileMethodInvocation<TResult>(Type type, string methodName, params Type[] methodSignature)
        {
            var typeInfo = type.GetTypeInfo();

            var methodInfo = methodSignature == null || !methodSignature.Any()
                ? typeInfo.GetMethods(BindingFlags.Instance | BindingFlags.Public).SingleOrDefault(m => m.Name == methodName)
                : typeInfo.GetMethod(methodName, methodSignature);

            if (methodInfo == null)
            {
                throw new ArgumentException($"Type '{type.PrettyPrint()}' doesn't have a method called '{methodName}'");
            }

            var genericArguments = typeof(TResult).GetTypeInfo().GetGenericArguments();
            var methodArgumentList = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();
            var funcArgumentList = genericArguments.Skip(1).Take(methodArgumentList.Count).ToList();

            if (funcArgumentList.Count != methodArgumentList.Count)
            {
                throw new ArgumentException("Incorrect number of arguments");
            }

            var instanceArgument = Expression.Parameter(genericArguments[0]); ;

            var argumentPairs = funcArgumentList.Zip(methodArgumentList, (s, d) => new { Source = s, Destination = d }).ToList();
            if (argumentPairs.All(a => a.Source == a.Destination))
            {
                // No need to do anything fancy, the types are the same
                var parameters = funcArgumentList.Select(Expression.Parameter).ToList();
                return Expression.Lambda<TResult>(Expression.Call(instanceArgument, methodInfo, parameters), new[] { instanceArgument }.Concat(parameters)).Compile();
            }

            var lambdaArgument = new List<ParameterExpression>
                {
                    instanceArgument,
                };
            var instanceVariable = Expression.Variable(type);
            var blockVariables = new List<ParameterExpression>
                {
                        instanceVariable,
                };
            var blockExpressions = new List<Expression>
                {
                    Expression.Assign(instanceVariable, Expression.ConvertChecked(instanceArgument, type))
                };
            var callArguments = new List<ParameterExpression>();

            foreach (var a in argumentPairs)
            {
                if (a.Source == a.Destination)
                {
                    var sourceParameter = Expression.Parameter(a.Source);
                    lambdaArgument.Add(sourceParameter);
                    callArguments.Add(sourceParameter);
                }
                else
                {
                    var sourceParameter = Expression.Parameter(a.Source);
                    var destinationVariable = Expression.Variable(a.Destination);
                    var assignToDestination = Expression.Assign(destinationVariable, Expression.Convert(sourceParameter, a.Destination));

                    lambdaArgument.Add(sourceParameter);
                    callArguments.Add(destinationVariable);
                    blockVariables.Add(destinationVariable);
                    blockExpressions.Add(assignToDestination);
                }
            }

            var callExpression = Expression.Call(instanceVariable, methodInfo, callArguments);
            blockExpressions.Add(callExpression);

            var block = Expression.Block(blockVariables, blockExpressions);

            var lambdaExpression = Expression.Lambda<TResult>(block, lambdaArgument);

            return lambdaExpression.Compile();
        }
    }
}
