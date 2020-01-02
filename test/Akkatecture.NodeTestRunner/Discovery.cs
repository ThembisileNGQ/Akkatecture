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
using Xunit;
using Xunit.Abstractions;

namespace Akkatecture.NodeTestRunner
{
#pragma warning disable 618
    public class Discovery : TestMessageVisitor<IDiscoveryCompleteMessage>
#pragma warning restore 618
    {
        private readonly string _assemblyName;
        private readonly string _className;
        public List<ITestCase> TestCases { get; private set; }

        public Discovery(string assemblyName, string className)
        {
            _assemblyName = assemblyName;
            _className = className;
            TestCases = new List<ITestCase>();
        }

        protected override bool Visit(ITestCaseDiscoveryMessage discovery)
        {
            var name = discovery.TestAssembly.Assembly.AssemblyPath.Split('\\').Last();
            if (!name.Equals(_assemblyName, StringComparison.OrdinalIgnoreCase))
                return true;

            var testName = discovery.TestClass.Class.Name;
            if (testName.Equals(_className, StringComparison.OrdinalIgnoreCase))
            {
                TestCases.Add(discovery.TestCase);
            }
            return true;
        }
    }
}