#!/bin/sh

rm -rf ./Akkatecture.Tests.MultiNode.AggregateClusterTests/
rm -rf ./Akkatecture.Tests.MultiNode.AggregateSagaClusterTests/
rm -rf ./bin/
rm -rf ./obj/

dotnet build Akkatecture.Tests.MultiNode.csproj --configuration Release
dotnet build ../Akkatecture.NodeTestRunner/Akkatecture.NodeTestRunner.csproj --configuration Release --runtime osx-x64
dotnet build ../Akkatecture.MultiNodeTestRunner/Akkatecture.MultiNodeTestRunner.csproj --configuration Release --runtime osx-x64
dotnet ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore
#/bin/cp -rf /bin/Release/netcoreapp2.2/Newtonsoft.Json.dll /tools/osx-x64/Akka.MultiNodeTestRunner

#dotnet ./tools/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore