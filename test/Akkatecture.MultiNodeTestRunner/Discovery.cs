// The MIT License (MIT)
//
// Copyright (c) 2009 - 2020 Lightbend Inc.
// Copyright (c) 2013 - 2020 .NET Foundation
// Modified from original source https://github.com/akkadotnet/akka.net
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
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
using System.Reflection;
using System.Threading;
using Akka.MultiNodeTestRunner.Shared;
using Akka.Remote.TestKit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Akkatecture.MultiNodeTestRunner
{
    public class Discovery : IMessageSink, IDisposable
    {
        public Dictionary<string, List<NodeTest>> Tests { get; set; }
        public List<ErrorMessage> Errors { get; } = new List<ErrorMessage>();
        public bool WasSuccessful => Errors.Count == 0;
        public Discovery()
        {
            Tests = new Dictionary<string, List<NodeTest>>();
            Finished = new ManualResetEvent(false);
        }

        public ManualResetEvent Finished { get; private set; }

        public virtual bool OnMessage(IMessageSinkMessage message)
        {
            switch (message)
            {
                case ITestCaseDiscoveryMessage testCaseDiscoveryMessage:
                    var testClass = testCaseDiscoveryMessage.TestClass.Class;
                    if (testClass.IsAbstract) return true;
                    var specType = testCaseDiscoveryMessage.TestAssembly.Assembly.GetType(testClass.Name).ToRuntimeType();

                    var roles = RoleNames(specType);

                    var details = roles.Select((r, i) => new NodeTest
                    {
                        Node = i + 1,
                        Role = r.Name,
                        TestName = testClass.Name,
                        TypeName = testClass.Name,
                        MethodName = testCaseDiscoveryMessage.TestCase.TestMethod.Method.Name,
                        SkipReason = testCaseDiscoveryMessage.TestCase.SkipReason,
                    }).ToList();
                    if (details.Any())
                    {
                        var dictKey = details.First().TestName;
                        if (Tests.ContainsKey(dictKey))
                            Tests[dictKey].AddRange(details);
                        else
                            Tests.Add(dictKey, details);
                    }
                    break;
                case IDiscoveryCompleteMessage _:
                    Finished.Set();
                    break;
                case ErrorMessage err:
                    Errors.Add(err);
                    break;
            }

            return true;
        }

        private IEnumerable<RoleName> RoleNames(Type specType)
        {
            var ctorWithConfig = FindConfigConstructor(specType);
            var configType = ctorWithConfig.GetParameters().First().ParameterType;
            var args = ConfigConstructorParamValues(configType);
            var configInstance = Activator.CreateInstance(configType, args);
            var roleType = typeof(RoleName);
            var configProps = configType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var roleProps = configProps.Where(p => p.PropertyType == roleType && p.Name != "Myself").Select(p => (RoleName)p.GetValue(configInstance));
            var configFields = configType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var roleFields = configFields.Where(f => f.FieldType == roleType && f.Name != "Myself").Select(f => (RoleName)f.GetValue(configInstance));
            var roles = roleProps.Concat(roleFields).Distinct();
            return roles;
        }

        internal static ConstructorInfo FindConfigConstructor(Type configUser)
        {
            var baseConfigType = typeof(MultiNodeConfig);
            var current = configUser;
            while (current != null)
            {
                var ctorWithConfig = current
                    .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(c => null != c.GetParameters().FirstOrDefault(p => p.ParameterType.GetTypeInfo().IsSubclassOf(baseConfigType)));

                current = current.GetTypeInfo().BaseType;
                if (ctorWithConfig != null) return ctorWithConfig;
            }

            throw new ArgumentException($"[{configUser}] or one of its base classes must specify constructor, which first parameter is a subclass of {baseConfigType}");
        }

        private object[] ConfigConstructorParamValues(Type configType)
        {
            var ctors = configType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var empty = ctors.FirstOrDefault(c => !c.GetParameters().Any());

            return empty != null
                ? new object[0]
                : ctors.First().GetParameters().Select(p => p.ParameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(p.ParameterType) : null).ToArray();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Finished.Dispose();
        }
    }
}
