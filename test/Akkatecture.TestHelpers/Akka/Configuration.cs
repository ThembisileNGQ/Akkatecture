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

namespace Akkatecture.TestHelpers.Akka
{
    public static class Configuration
    {
        public static string Config =
            @"  akka.loglevel = ""DEBUG""
                akka.stdout-loglevel = ""DEBUG""
                akka.actor.serialize-messages = on
                akka.persistence.snapshot-store {
                    plugin = ""akka.persistence.snapshot-store.inmem""
                    # List of snapshot stores to start automatically. Use "" for the default snapshot store.
                    auto-start-snapshot-stores = []
                }
                akka.persistence.snapshot-store.inmem {
                    # Class name of the plugin.
                    class = ""Akka.Persistence.Snapshot.MemorySnapshotStore, Akka.Persistence""
                    # Dispatcher for the plugin actor.
                    plugin-dispatcher = ""akka.actor.default-dispatcher""
                }
            ";
    }
}