#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open Fake.Core
open Fake.Api
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.BuildServer
open Fake.Testing
open FSharp.Json

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
    | PR

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
                    | Some "pr" -> Some PR
                    | Some "alpha" -> Some Alpha
                    | Some "prerelease" -> Some PreRelease
                    | Some "nuget" -> Some NuGet
                    | _ -> None

let buildNumber = 
    match host with
        | Local -> "0.0.1"
        | AzureDevOps -> Environment.environVarOrFail "BUILD_BUILDNUMBER"

let branch = 
    match envOrNone "BRANCH" with 
        | Some "master" -> "master"
        | Some _ -> "dev"
        | None -> "dev"

let shouldScan = hasEnv "SONARCLOUD_TOKEN" && host <> Local
let canPush = hasEnv "NUGET_FEED_PAT" || hasEnv "INTERNAL_FEED_PAT"
let canGithubRelease = hasEnv "GITHUB_PAT"

let runtimeIds = dict[Windows, "win-x64"; Linux, "linux-x64"; OSX, "osx-x64"]
let runtimeId = runtimeIds.Item(platform);
let configuration = DotNet.BuildConfiguration.Release
let solution = IO.Path.GetFullPath(string "../Akkatecture.sln")
let sourceDirectory =  IO.Path.GetFullPath(string "../")
let sonarqubeDirectory = sourceDirectory @@ ".sonarqube"
let toolsDirectory = sourceDirectory @@ "build" @@ "tools"
let artefactsDirectory = sourceDirectory @@ "artefacts"
let coverageResults = sourceDirectory @@ "coverageresults"
let multiNodeLogs = sourceDirectory @@ "multinodelogs"
let multiNodeTestScript = sourceDirectory @@ "build" @@ "Run-MultiNodeTests.ps1"
let feed = lazy(env "FEEDVERSION")
let internalCredential = lazy ({ Endpoint = sprintf "https://pkgs.dev.azure.com/lutando/Akkatecture/_packaging/%s/nuget/v3/index.json" feed.Value; Username = "lutando"; Password = env "INTERNAL_FEED_PAT"})
let nugetCredential = lazy ({ Endpoint = "https://api.nuget.org/v3/index.json"; Username = "lutando"; Password = env "NUGET_FEED_PAT"})
let sonarQubeKey =  lazy (env "SONARCLOUD_TOKEN")
let githubKey = lazy (env "GITHUB_PAT")

let endpointCredentials : EndpointCredentials = 
    let credentials = 
        seq { if hasEnv "NUGET_FEED_PAT" then
                yield nugetCredential.Value
              if hasEnv "INTERNAL_FEED_PAT" then
                 yield internalCredential.Value }
    
    {EndpointCredentials = Seq.toList credentials}

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
        ++ coverageResults
        ++ toolsDirectory
        ++ sonarqubeDirectory
        ++ artefactsDirectory
  
    cleanables |> Shell.cleanDirs

    File.delete multiNodeTestScript

    Trace.log " --- Build Variables ---"
    Trace.logfn "Platform: %A" platform
    Trace.logfn "RuntimeId: %s" runtimeId
    Trace.logfn "Host: %A" host
    Trace.logfn "BuildNumber: %s" buildNumber
    Trace.logfn "Home: %s" (env "HOME")
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

        let sonarLogin = sprintf "sonar.login=%s" sonarQubeKey.Value;
        let sonarReportPaths = sprintf "sonar.cs.opencover.reportsPaths=\"%s\",\"%s\"" (coverageResults </> "unit.opencover.xml") (coverageResults </> "multinode.opencover.xml");
        let sonarBranchName = sprintf "sonar.branch.name=%s" branch
        let sonarQubeOptions (defaults:SonarQube.SonarQubeParams) =
            {defaults with
                ToolsPath = toolsDirectory </> "dotnet-sonarscanner"
                Key = "Lutando_Akkatecture"
                Name = "Akkatecture"
                Version = buildNumber
                Organization = Some "lutando-github"
                Settings = [
                    "sonar.verbose=true";
                    "sonar.host.url=https://sonarcloud.io/";
                    sonarBranchName;
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

    let copyBinaries = 
        Shell.mkdir artefactsDirectory
        let packagesGlob =  !! (sprintf "src/**/bin/%A/*.nupkg" configuration)
        packagesGlob |> Seq.iter (Shell.copyFile artefactsDirectory)

    copyBinaries
    
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
                        "CoverletOutputFormat", "opencover"
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
    let mergable = (coverageResults </> "unit.opencover.json")
    
    Shell.copyFile target file

    if File.exists multiNodeTestScript then
        File.delete multiNodeTestScript
    
    let coverletCommand = toolsDirectory </> "coverlet"
    let coverletArgs = sprintf "'%s' --target='dotnet' --targetargs='%s %s -Dmultinode.platform=netcore -Dmultinode.output-directory=%s' --format='opencover' --include='[Akkatecture]' --include='[Akkatecture.Clustering]' --exclude='[xunit*]*' --exclude='[Akka.NodeTestRunner*]*' --exclude='[Akkatecture.NodeTestRunner*]*' --verbosity='detailed' --output='%s' --merge-with='%s'" testRunnerDll testRunnerDll testsDll multiNodeLogs results mergable
    let expression = sprintf "Invoke-Expression \"%s %s\"" coverletCommand coverletArgs
    
    Trace.log expression

    File.write false multiNodeTestScript [expression]

    let execution = Shell.Exec (cmd = "pwsh", args = multiNodeTestScript)

    match execution with 
        | 0 -> Trace.log  "MultiNodeTests passed."
        | message -> failwithf "MultiNodeTests failed with %i:" message
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
                    sprintf "sonar.login=%s" sonarQubeKey.Value;]}

        SonarQube.finish (Some sonarQubeOptions)
)

Target.create "Push" (fun _ ->
    Trace.log " --- Publish Packages --- "

    match host with
        | AzureDevOps _ -> ()
        | Local -> Trace.logfn "NugetFeedUrls: %s" (Json.serialize endpointCredentials)

    match feedVersion with
        | Some fv -> Trace.logfn "FeedVersion: %A" fv
        | None -> ()

    if canPush then
        match feedVersion with 
            | Some NuGet  -> ()
            | Some _ -> installCredentialProvider sourceDirectory endpointCredentials
            | None -> ()


        let source = match feedVersion with
                        | Some NuGet -> Some nugetCredential.Value.Endpoint
                        | Some _ -> Some internalCredential.Value.Endpoint
                        | _ -> None

        let apiKey = match feedVersion with
                        | Some NuGet -> Some nugetCredential.Value.Password
                        | Some _ -> Some internalCredential.Value.Password
                        | _ -> None

        let packagesGlob = sprintf "%s/*.nupkg" artefactsDirectory

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
    
    if canGithubRelease then
        let releaseNotes = 
            seq {
                yield "*see the [changelog](https://github.com/Lutando/Akkatecture/blob/dev/CHANGELOG.md) for all other release information*"
            } 
        GitHub.createClientWithToken githubKey.Value
        |> GitHub.draftNewRelease "Lutando" "Akkatecture" buildNumber false releaseNotes
        |> GitHub.publishDraft
        |> Async.RunSynchronously


)

// --------------------------------------------------------------------------------------
// Build order
// --------------------------------------------------------------------------------------

Target.create "Release" DoNothing
Target.create "Default" DoNothing

"Clean"
  ==> "Restore"
  ==> "SonarQubeStart"
  ?=> "Build"
  ==> "Test"
  ==> "MultiNodeTest"
  ==> "SonarQubeEnd"
  ?=> "Push"
  ==> "Release"
  ==> "GitHubRelease"

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "MultiNodeTest"
  ==> "Default"

Target.runOrDefault "Default"
