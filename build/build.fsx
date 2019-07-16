#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open Fake.Core
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
        | Local -> "0.0.2"
        | AzureDevOps -> Environment.environVarOrFail "BUILD_BUILDNUMBER"

//Todo make variables lazy so that they wont be evaluated in production builds
let runtimeIds = dict[Windows, "win-x64"; Linux, "linux-x64"; OSX, "osx-x64"]
let runtimeId = runtimeIds.Item(platform);
let configuration = DotNet.BuildConfiguration.Release
let solution = IO.Path.GetFullPath(string "../Akkatecture.sln")
let sourceDirectory =  IO.Path.GetFullPath(string "../")
let toolsDirectory = sourceDirectory @@ "build" @@ "tools"
let testResults = sourceDirectory @@ "testresults"
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
        ++ testResults
        ++ toolsDirectory
        
    cleanables |> Shell.cleanDirs   

    Trace.log " --- Build Variables ---"
    Trace.logfn "Platform: %A" platform
    Trace.logfn "RuntimeId: %s" runtimeId
    Trace.logfn "Host: %A" host
    Trace.logfn "BuildNumber: %s" buildNumber
    Trace.logfn "Home: %s" (env "HOME")
    Trace.logfn "NugetFeedUrls: %s" (Json.serialize endpointCredentials)
    match feedVersion with
        | Some fv -> Trace.logfn "FeedVersion: %A" fv
        | None -> ()
)

Target.create "Restore" (fun _ ->
    Trace.log " --- Restoring Solution --- "

    DotNet.restore id solution
)

Target.create "SonarQubeStart" (fun _ ->
    Trace.log " --- Sonar Qube Starting --- "

    installSonarScanner toolsDirectory

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
                sprintf "sonar.login=%s" sonarQubeKey;
                sprintf "sonar.cs.opencover.reportsPaths=\"%s\"" testResults </> "opencover.xml";
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
    let coverletOutput = testResults </> "opencover.xml"
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
)

Target.create "SonarQubeEnd" (fun _ ->
    Trace.log " --- Sonar Qube Ending --- "
    
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
  ==> "Test"
  ==> "SonarQubeEnd"
  ==> "Push"
  ==> "GitHubRelease"
  ==> "Release"

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "Default"

Target.runOrDefault "Build"
