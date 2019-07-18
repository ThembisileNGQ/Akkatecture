#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open System.Diagnostics
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.BuildServer
open Fake.Testing
open FSharp.Json
open BlackFox.CommandLine

Target.initEnvironment()
let DoNothing = ignore
let vault = 
    match Vault.fromFakeEnvironmentOrNone() with
        | Some v -> v
        | None -> TeamFoundation.variables

let env value =
    match vault.TryGet value with
        | Some v -> v
        | None -> Environment.environVarOrFail value

let envOrNone value = 
    match vault.TryGet value with
        | Some v -> Some v
        | None -> Environment.environVarOrNone value

let hasEnv value = 
    match vault.TryGet value with
        | Some _ -> true
        | None -> Environment.hasEnvironVar value

let installCredentialProvider sourceDirectory endpointCredentials =
    let script = sourceDirectory </> "build/installcredprovider.sh"
    let execution = Shell.Exec ("sh", script )
    
    match execution with 
        | 0 -> printf "NuGet Credential Provider installed"
        | _ -> failwith "NuGet Credential Provider failed to install"

    Environment.setEnvironVar "VSS_NUGET_EXTERNAL_FEED_ENDPOINTS" (Json.serialize endpointCredentials)
    Trace.logfn "Nugetfeedurls: %s" (env "VSS_NUGET_EXTERNAL_FEED_ENDPOINTS")

let installSonarScanner toolsDirectory =
    let arg = sprintf "tool install dotnet-sonarscanner --tool-path %s" toolsDirectory

    let execution = Shell.Exec (cmd = "dotnet", args = arg) 

    match execution with 
        | 0 -> Trace.log  "SonarScanner installed"
        | _ -> failwith "SonarScanner failed to install"

let installCoverlet toolsDirectory =
    let arg = sprintf "tool install coverlet.console --tool-path %s" toolsDirectory

    let execution = Shell.Exec (cmd = "dotnet", args = arg) 

    match execution with 
        | 0 -> Trace.log  "Coverlet Console installed"
        | _ -> failwith "Coverlet Console failed to install"

let runProc filename args startDir = 
    let timer = Stopwatch.StartNew()
    let procStartInfo = 
        ProcessStartInfo(
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            FileName = filename,
            Arguments = args
        )
    match startDir with | Some d -> procStartInfo.WorkingDirectory <- d | _ -> ()

    let outputs = System.Collections.Generic.List<string>()
    let errors = System.Collections.Generic.List<string>()
    let outputHandler f (_sender:obj) (args:DataReceivedEventArgs) = f args.Data
    let p = new Process(StartInfo = procStartInfo)
    p.OutputDataReceived.AddHandler(DataReceivedEventHandler (outputHandler outputs.Add))
    p.ErrorDataReceived.AddHandler(DataReceivedEventHandler (outputHandler errors.Add))
    let started = 
        try
            p.Start()
        with | ex ->
            ex.Data.Add("filename", filename)
            reraise()
    if not started then
        failwithf "Failed to start process %s" filename
    printfn "Started %s with pid %i" p.ProcessName p.Id
    p.BeginOutputReadLine()
    p.BeginErrorReadLine()
    p.WaitForExit()
    timer.Stop()
    printfn "Finished %s after %A milliseconds" filename timer.ElapsedMilliseconds
    let cleanOut l = l |> Seq.filter (fun o -> String.IsNullOrEmpty o |> not)
    cleanOut outputs,cleanOut errors

// --------------------------------------------------------------------------------------
// Build types
// --------------------------------------------------------------------------------------

type Platform =
    | Windows
    | Linux
    | OSX

type Host =
    | Local
    | AzureDevOps

type FeedVersion =
    | Fake
    | Alpha
    | PreRelease
    | NuGet

type EndpointCredential = 
    { [<JsonField("endpoint")>]Endpoint: string 
      [<JsonField("username")>]Username: string
      [<JsonField("password")>]Password: string }
and EndpointCredentials =  
    { [<JsonField("endpointCredentials")>]EndpointCredentials : EndpointCredential list }

// --------------------------------------------------------------------------------------
// Build variables
// --------------------------------------------------------------------------------------
                    
let host = match TeamFoundation.detect() with
            | true -> AzureDevOps
            | false -> Local

let platform =
    if Environment.isMacOS then OSX
    elif Environment.isLinux then Linux
    else Windows

let feedVersion = match envOrNone "FEEDVERSION" with
                    | Some "fake" -> Some Fake
                    | Some "alpha" -> Some Alpha
                    | Some "prerelease" -> Some PreRelease
                    | Some "release" -> Some NuGet
                    | _ -> None

let buildNumber = 
    match host with
        | Local -> "0.0.1"
        | AzureDevOps -> Environment.environVarOrFail "BUILD_BUILDNUMBER"

let shouldScan = hasEnv "SONARCLOUD_TOKEN" && host <> Local

//Todo make variables lazy so that they wont be evaluated in production builds
let runtimeIds = dict[Windows, "win-x64"; Linux, "linux-x64"; OSX, "osx-x64"]
let runtimeId = runtimeIds.Item(platform);
let configuration = DotNet.BuildConfiguration.Release
let solution = IO.Path.GetFullPath(string "../Akkatecture.sln")
let sourceDirectory =  IO.Path.GetFullPath(string "../")
let archiveDirectory = sourceDirectory @@ "archive"
let sonarqubeDirectory = sourceDirectory @@ ".sonarqube"
let toolsDirectory = sourceDirectory @@ "build" @@ "tools"
let coverageResults = sourceDirectory @@ "coverageresults"
let multiNodeLogs = sourceDirectory @@ "multinodelogs"
let internalCredential = { Endpoint = "https://pkgs.dev.azure.com/lutando/_packaging/akkatecture/nuget/v3/index.json"; Username = "lutando"; Password = env "INTERNAL_FEED_PAT"}
let nugetCredential = { Endpoint = "https://api.nuget.org/v3/index.json"; Username = "lutando"; Password = env "NUGET_FEED_PAT"}
let sonarQubeKey = env "SONARCLOUD_TOKEN"
let endpointCredentials : EndpointCredentials = { EndpointCredentials = [internalCredential; nugetCredential] }

// --------------------------------------------------------------------------------------
// Build Current Working Directory
// --------------------------------------------------------------------------------------

Environment.CurrentDirectory <- sourceDirectory

// --------------------------------------------------------------------------------------
// Build Targets
// --------------------------------------------------------------------------------------

Target.create "Clean" (fun _ ->
    Trace.log " --- Cleaning Solution --- "

    let cleanables = 
        !! "src/**/bin"
        ++ "src/**/obj"
        ++ "test/**/bin"
        ++ "test/**/obj"
        ++ "examples/**/bin"
        ++ "examples/**/obj"
        ++ multiNodeLogs
        ++ archiveDirectory
        ++ coverageResults
        ++ toolsDirectory
        ++ sonarqubeDirectory

        
    cleanables |> Shell.cleanDirs   

    Trace.log " --- Build Variables ---"
    Trace.logfn "Platform: %A" platform
    Trace.logfn "RuntimeId: %s" runtimeId
    Trace.logfn "Host: %A" host
    Trace.logfn "BuildNumber: %s" buildNumber
    Trace.logfn "Home: %s" (env "HOME")
    
    match host with
        | AzureDevOps _ -> ()
        | Local -> Trace.logfn "NugetFeedUrls: %s" (Json.serialize endpointCredentials)

    match feedVersion with
        | Some fv -> Trace.logfn "FeedVersion: %A" fv
        | None -> ()
)

Target.create "Archive" (fun _ ->
    Trace.log " --- Archiving Solution --- "

)

Target.create "Restore" (fun _ ->
    Trace.log " --- Restoring Solution --- "

    DotNet.restore id solution
)

Target.create "SonarQubeStart" (fun _ ->
    Trace.log " --- Sonar Qube Starting --- "

    if shouldScan then

        installSonarScanner toolsDirectory

        let sonarLogin = sprintf "sonar.login=%s" sonarQubeKey;
        let sonarReportPaths = sprintf "sonar.cs.opencover.reportsPaths=\"%s\",\"%s\"" (coverageResults </> "unit.opencover.xml") (coverageResults </> "multinode.opencover.xml");
        let sonarQubeOptions (defaults:SonarQube.SonarQubeParams) =
            {defaults with
                ToolsPath = toolsDirectory </> "dotnet-sonarscanner"
                Key = "Lutando_Akkatecture"
                Name = "Akkatecture"
                Version = buildNumber
                Settings = [
                    "sonar.verbose=true /o:lutando-github";
                    "sonar.host.url=https://sonarcloud.io/";
                    "sonar.branch.name=dev";
                    sonarLogin;
                    sonarReportPaths;
                    "sonar.visualstudio.enable=false"]}

        SonarQube.start sonarQubeOptions

)

Target.create "Build" (fun _ ->
    Trace.log " --- Building Projects --- "

    let projects = 
        !! "src/**/*.*proj"
        ++ "test/Akkatecture.Tests/Akkatecture.Tests.csproj"

    let buildOptions (defaults:DotNet.BuildOptions) =
        { defaults with
            MSBuildParams = 
                { defaults.MSBuildParams with
                    Properties = ["Version", buildNumber] }
            NoRestore = true
            Configuration = configuration }

    projects |> Seq.iter (DotNet.build buildOptions)
    
)

Target.create "Test" (fun _ ->
    Trace.log " --- Unit Tests --- "

    let projects = !! "test/Akkatecture.Tests/Akkatecture.Tests.csproj"
    let coverletOutput = coverageResults </> "unit.opencover.xml"
    let testOptions (defaults:DotNet.TestOptions) =
        { defaults with
            MSBuildParams = 
                { defaults.MSBuildParams with
                    Properties = [
                        "CollectCoverage", "true";
                        "CoverletOutputFormat", "opencover";
                        "CoverletOutput", coverletOutput;
                        "Exclude", @"[xunit*]*,[Akkatecture.TestHelpers]*,[Akkatecture.Tests*]*,[*TestRunner*]*"] }
            Configuration = configuration
            NoBuild = true}

    projects |> Seq.iter (DotNet.test testOptions)
)

Target.create "MultiNodeTest" (fun _ ->
    Trace.log " --- Multi Node Tests --- "

    installCoverlet toolsDirectory

    let multiNodeTestProjects = !! "test/Akkatecture.Tests.MultiNode/Akkatecture.Tests.MultiNode.csproj"

    let multiNodeTestbuildOptions (defaults:DotNet.BuildOptions) =
        { defaults with
            Configuration = configuration }

    multiNodeTestProjects |> Seq.iter (DotNet.build multiNodeTestbuildOptions)

    let nodeTestRunnerProjects = 
        !! "test/Akkatecture.NodeTestRunner/Akkatecture.NodeTestRunner.csproj"
        ++ "test/Akkatecture.MultiNodeTestRunner/Akkatecture.MultiNodeTestRunner.csproj"

    let multiNodeRunnerbuildOptions (defaults:DotNet.BuildOptions) =
        { defaults with
            Configuration = configuration 
            Runtime = Some runtimeId }

    nodeTestRunnerProjects |> Seq.iter (DotNet.build multiNodeRunnerbuildOptions)

    let testRunnerBinaryFolder = sourceDirectory @@ "test" @@ "Akkatecture.MultiNodeTestRunner" @@ "bin" @@ configuration.ToString() @@ "netcoreapp2.2" @@ runtimeId
    let testRunnerDll = testRunnerBinaryFolder @@ "Akka.MultiNodeTestRunner.dll"
    let testsBinaryFolder = sourceDirectory @@ "test" @@ "Akkatecture.Tests.MultiNode" @@ "bin" @@ configuration.ToString() @@ "netcoreapp2.2"
    let testsDll = testsBinaryFolder @@ "Akkatecture.Tests.MultiNode.dll"
    let target = testRunnerBinaryFolder @@ "Newtonsoft.Json.dll"
    let file = testsBinaryFolder @@ "Newtonsoft.Json.dll"
    let results =(coverageResults </> "multinode.opencover.xml")
    
    Shell.copyFile target file
    
    let coverletCommand = toolsDirectory </> "coverlet"
    let coverletArgs = sprintf "'%s' --target 'dotnet' --targetargs \"'%s %s -Dmultinode.platform=netcore -Dmultinode.output-directory=%s'\" --format 'opencover' --include=\"[Akkatecture]\" --include=\"[Akkatecture.Clustering]\" --exclude=\"[xunit*]*\" --exclude=\"[Akka.NodeTestRunner*]*\" --exclude=\"[Akkatecture.NodeTestRunner*]*\" --verbosity='detailed' --output'%s'" testRunnerDll testRunnerDll testsDll multiNodeLogs results
    Trace.log "coverlet command"
    Trace.log coverletCommand
    Trace.log "coverlet args"
    Trace.log coverletArgs
    let execution = Shell.Exec (cmd = coverletCommand, args = coverletArgs) 
//https://stackoverflow.com/questions/192249/how-do-i-parse-command-line-arguments-in-bash



    match execution with 
        | 0 -> Trace.log  "MultiNodeTests passed."
        | message -> failwithf "MultiNodeTests failed with %i:" message

//dotnet build Akkatecture.Tests.MultiNode.csproj --configuration Release
//dotnet build ../Akkatecture.NodeTestRunner/Akkatecture.NodeTestRunner.csproj --configuration Release --runtime osx-x64
//dotnet build ../Akkatecture.MultiNodeTestRunner/Akkatecture.MultiNodeTestRunner.csproj --configuration Release --runtime osx-x64
//bin/cp -rf ./bin/Release/netcoreapp2.2/Newtonsoft.Json.dll ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Newtonsoft.Json.dll
//#dotnet ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore
//coverlet '../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll' --target 'dotnet' --targetargs ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore -Dmultinode.output-directory=/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/multinodelogs' --format 'opencover' --include "[Akkatecture]" --include "[Akkatecture.Clustering]" --exclude "[xunit*]*" --exclude "[Akka.NodeTestRunner*]*" --exclude "[Akkatecture.NodeTestRunner*]*" --verbosity detailed --output '/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/coverageresults/multinode.opencover.xml'

)

Target.create "MultiNodeTestPoC" (fun _ ->
    Trace.log " --- Multi Node Tests --- "

    let testRunnerBinaryFolder = sourceDirectory @@ "test" @@ "Akkatecture.MultiNodeTestRunner" @@ "bin" @@ configuration.ToString() @@ "netcoreapp2.2" @@ runtimeId
    let testRunnerDll = testRunnerBinaryFolder @@ "Akka.MultiNodeTestRunner.dll"
    let testsBinaryFolder = sourceDirectory @@ "test" @@ "Akkatecture.Tests.MultiNode" @@ "bin" @@ configuration.ToString() @@ "netcoreapp2.2"
    let testsDll = testsBinaryFolder @@ "Akkatecture.Tests.MultiNode.dll"
    let target = testRunnerBinaryFolder @@ "Newtonsoft.Json.dll"
    let file = testsBinaryFolder @@ "Newtonsoft.Json.dll"
    
    Shell.copyFile target file
    
    let coverletCommand =  toolsDirectory </> "coverlet"
    let coverletArgs = sprintf "--target 'dotnet' --targetargs='%s %s -Dmultinode.platform=netcore -Dmultinode.output-directory=%s' --format='opencover' --include=\"[Akkatecture]\" --include=\"[Akkatecture.Clustering]\" --exclude=\"[xunit*]*\" --exclude=\"[Akka.NodeTestRunner*]*\" --exclude=\"[Akkatecture.NodeTestRunner*]*\" --verbosity detailed --output '%s'" testRunnerDll testsDll multiNodeLogs (coverageResults </> "multinode.opencover.xml")
    //Trace.log "coverlet command"
    //Trace.log coverletCommand
    //Trace.log "coverlet args"
    //Trace.log coverletArgs
    
    CmdLine.empty
    |> CmdLine.append testRunnerDll
    |> CmdLine.append (sprintf "--target=dotnet %s" (sprintf "--verbosity=detailed --targetargs=%s '%s'" testRunnerDll testsDll))
    //|> CmdLine.append (sprintf "--target='dotnet' %s" (sprintf "--targetargs=%s %s --format='opencover'" testRunnerDll testsDll))
    //|> CmdLine.append (sprintf "--targetargs='%s %s'" testRunnerDll testsDll)
    //|> CmdLine.append @"--format='opencover'"
    |> CmdLine.toString
    //|> Trace.log
    |> CreateProcess.fromRawCommandLine coverletCommand
    |> Proc.run
    |> ignore

    
//dotnet build Akkatecture.Tests.MultiNode.csproj --configuration Release
//dotnet build ../Akkatecture.NodeTestRunner/Akkatecture.NodeTestRunner.csproj --configuration Release --runtime osx-x64
//dotnet build ../Akkatecture.MultiNodeTestRunner/Akkatecture.MultiNodeTestRunner.csproj --configuration Release --runtime osx-x64
//bin/cp -rf ./bin/Release/netcoreapp2.2/Newtonsoft.Json.dll ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Newtonsoft.Json.dll
//#dotnet ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore
//coverlet '../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll' --target 'dotnet' --targetargs ../Akkatecture.MultiNodeTestRunner/bin/Release/netcoreapp2.2/osx-x64/Akka.MultiNodeTestRunner.dll ./bin/Release/netcoreapp2.2/Akkatecture.Tests.MultiNode.dll -Dmultinode.platform=netcore -Dmultinode.output-directory=/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/multinodelogs' --format 'opencover' --include "[Akkatecture]" --include "[Akkatecture.Clustering]" --exclude "[xunit*]*" --exclude "[Akka.NodeTestRunner*]*" --exclude "[Akkatecture.NodeTestRunner*]*" --verbosity detailed --output '/Users/lutandongqakaza/Workspace/Akkatecture/Akkatecture/coverageresults/multinode.opencover.xml'

)

Target.create "SonarQubeEnd" (fun _ ->
    Trace.log " --- Sonar Qube Ending --- "

    if shouldScan then

        let sonarQubeOptions (defaults:SonarQube.SonarQubeParams) =
            {defaults with
                ToolsPath = toolsDirectory </> "dotnet-sonarscanner"
                Key = "Lutando_Akkatecture"
                Name = "Akkatecture"
                Version = buildNumber
                Settings = [
                    sprintf "sonar.login=%s" sonarQubeKey;]}

        SonarQube.finish (Some sonarQubeOptions)
)

Target.create "Push" (fun _ ->
    Trace.log " --- Publish Packages --- "

    match feedVersion with 
        | Some NuGet  -> ()
        | Some _ -> installCredentialProvider sourceDirectory endpointCredentials
        | None -> ()


    let source = match feedVersion with
                    | Some NuGet -> Some nugetCredential.Endpoint
                    | Some _ -> Some internalCredential.Endpoint
                    | _ -> None

    let apiKey = match feedVersion with
                    | Some NuGet -> Some nugetCredential.Password
                    | Some _ -> Some internalCredential.Password
                    | _ -> None

    let packagesGlob = sprintf "src/**/bin/%A/*.nupkg" configuration

    let nugetPushParams (defaults:NuGet.NuGet.NuGetPushParams) =
        { defaults with
            Source = source
            ApiKey = apiKey }
            
    let nugetPushOptions (defaults:DotNet.NuGetPushOptions) =
        { defaults with
            PushParams =  nugetPushParams defaults.PushParams }

    let packages =
        !! packagesGlob

    packages |> Seq.iter (DotNet.nugetPush nugetPushOptions)
)

Target.create "GitHubRelease" (fun _ ->
    Trace.log " --- GitHubRelease --- "


)

// --------------------------------------------------------------------------------------
// Build order
// --------------------------------------------------------------------------------------

Target.create "Release" DoNothing
Target.create "Default" DoNothing

"Clean"
  ==> "Restore"
  ==> "SonarQubeStart"
  ==> "Build"
  ==> "MultiNodeTest"
  ==> "Test"
  ==> "SonarQubeEnd"
  ==> "Push"
  ==> "Release"

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  //==> "MultiNodeTest"
  ==> "Default"

"Clean"
  ==> "Archive"
  ==> "Restore"
  ==> "SonarQubeStart"
  ==> "Build"
  ==> "Test"
  ==> "SonarQubeEnd"
  ==> "Push"
  ==> "GitHubRelease"

"MultiNodeTestPoC"

Target.runOrDefault "Build"
