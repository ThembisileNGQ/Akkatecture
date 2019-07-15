#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Testing
open Fake.BuildServer
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
    { endpointCredentials : EndpointCredential list }

// --------------------------------------------------------------------------------------
// Build variables
// --------------------------------------------------------------------------------------
Trace.logfn "// --------------------------------------------------------------------------------------"
Trace.logfn "// Build variables"
Trace.logfn "// --------------------------------------------------------------------------------------"
let test = match envOrNone "nugetfeedpat" with
            | Some s -> Trace.logfn "IT WAS nugetfeedpat %s" s
            | None -> match envOrNone "NUGETFEEDPAT" with
                        | Some s -> Trace.logfn "IT WAS NUGETFEEDPAT %s" s
                        | None -> match envOrNone "NUGET_FEED_PAT" with
                                    | Some s -> Trace.logfn "IT WAS NUGET_FEED_PAT %s" s
                                    | None -> Trace.log "IT WAS NOTHING"

let test2 = match envOrNone "TEST" with
            | Some s -> Trace.logfn "IT WAS TEST %s" s
            | None -> match envOrNone "test" with
                        | Some s -> Trace.logfn "IT WAS test %s" s
                        | None -> Trace.log "IT WAS NOTHING"
                    
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
    let dayOfYear = DateTime.UtcNow.DayOfYear.ToString() 
    let numberTemplate major minor patch feed revision = sprintf "%s.%s.%s-%s-%s%s" major minor patch feed revision
    match host with
        | Local -> "0.0.101"
        | AzureDevOps -> Environment.environVarOrFail "BUILD_BUILDNUMBER"//numberTemplate (env "MAJORVERSION") (env "MINORVERSION") (env "PATCHVERSION") (env "FEEDVERSION") dayOfYear (env "REVISION")

let runtimeIds = dict[Windows, "win-x64"; Linux, "linux-x64"; OSX, "osx-x64"]
let runtimeId = runtimeIds.Item(platform);
let configuration = DotNet.BuildConfiguration.Release
let solution = IO.Path.GetFullPath(string "../Akkatecture.sln")
let sourceDirectory =  IO.Path.GetFullPath(string "../")
let testResults = sourceDirectory @@ "testresults"
let pushesToFeed = match host with 
                    | AzureDevOps -> true
                    | _ -> false

let internalCredential = {Endpoint = "https://pkgs.dev.azure.com/lutando/_packaging/akkatecture/nuget/v3/index.json"; Username = "lutando"; Password = env "internalfeedpat"}
let nugetCredential = {Endpoint = "https://api.nuget.org/v3/index.json"; Username = "lutando"; Password = env "internalfeedpat"}

let endpointCredentials : EndpointCredentials = {endpointCredentials = [internalCredential;nugetCredential]}

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
                    Properties = ["CollectCoverage", "true"; "CoverletOutputFormat", "opencover"; "CoverletOutput", coverletOutput; "Exclude", @"[xunit*]*,[Akkatecture.TestHelpers]*,[Akkatecture.Tests*]*,[*TestRunner*]*"] }
            Configuration = configuration
            NoBuild = true}

    projects |> Seq.iter (DotNet.test testOptions)
)

Target.create "MultiNodeTest" (fun _ ->
    Trace.log " --- Multi Node Tests --- "
)

Target.create "SonarQubeEnd" (fun _ ->
    Trace.log " --- Sonar Qube Ending --- "
    //SonarQube.finish (Some (fun p -> {p with Settings = ["sonar.login=login"; "sonar.password=password"] }))
)

Target.create "Push" (fun _ ->
    Trace.log " --- Publish Packages --- "

    let script = sourceDirectory </> "build/installcredprovider.sh"
    let execution = Shell.Exec ("sh", script )
    
    match execution with 
        | 0 -> printf "NuGet Credential Provider installed"
        | _ -> failwith "NuGet Credential Provider failed to install"

    Environment.setEnvironVar "VSS_NUGET_EXTERNAL_FEED_ENDPOINTS" (Json.serialize endpointCredentials)
    Trace.logfn "Nugetfeedurls: %s" (env "VSS_NUGET_EXTERNAL_FEED_ENDPOINTS")


    let glob = sprintf "src/**/bin/%A/*.nupkg" configuration
    let nugetPushParams (defaults:NuGet.NuGet.NuGetPushParams) =
        { defaults with
            Source = Some "https://pkgs.dev.azure.com/lutando/_packaging/akkatecture/nuget/v3/index.json"
            ApiKey = Some "VSTS" }
            
    let nugetPushOptions (defaults:DotNet.NuGetPushOptions) =
        { defaults with
            PushParams =  nugetPushParams defaults.PushParams}

    let pushables =
        !! glob

    pushables |> Seq.iter (DotNet.nugetPush nugetPushOptions)
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
 // ==> "Test"
  ==> "SonarQubeEnd"
  ==> "Push"
  ==> "Release"

"Clean"
  ==> "Restore"
  ==> "Build"
//  ==> "Test"
  ==> "Default"

Target.runOrDefault "Build"
