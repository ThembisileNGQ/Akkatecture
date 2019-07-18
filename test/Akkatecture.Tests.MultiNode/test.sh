#!/bin/sh

#Clean
rm -rf ./Akkatecture.Tests.MultiNode.AggregateClusterTests/
rm -rf ./Akkatecture.Tests.MultiNode.AggregateSagaClusterTests/
rm -rf ./bin/
rm -rf ./obj/

dotnet build Akkatecture.Tests.MultiNode.csproj --configuration Release
dotnet build ../Akkatecture.NodeTestRunner/Akkatecture.NodeTestRunner.csproj --configuration Release --runtime osx-x64
dotnet build ../Akkatecture.MultiNodeTestRunner/Akkatecture.MultiNodeTestRunner.csproj --configuration Release --runtime osx-x64
/bin/cp -rf ./bin/Release/netcoreapp2.2/Newtonsoft.Json.dll ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Newtonsoft.Json.dll
#dotnet ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore
coverlet '../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll' --target 'dotnet' --targetargs '../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore -Dmultinode.output-directory=/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/multinodelogs' --format 'opencover' --include "[Akkatecture]" --include "[Akkatecture.Clustering]" --exclude "[xunit*]*" --exclude "[Akka.NodeTestRunner*]*" --exclude "[Akkatecture.NodeTestRunner*]*" --verbosity detailed --output '/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/coverageresults/multinode.opencover.xml'




/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/build/tools/coverlet '/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/test/Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll' --target 'dotnet' --targetargs '/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/test/Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll /Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/test/Akkatecture.Tests.MultiNode/bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore -Dmultinode.output-directory=/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/multinodelogs' --format 'opencover' --include "[Akkatecture]" --include "[Akkatecture.Clustering]" --exclude "[xunit*]*" --exclude "[Akka.NodeTestRunner*]*" --exclude "[Akkatecture.NodeTestRunner*]*" --verbosity detailed --output '/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/coverageresults/multinode.opencover.xml'