#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.BuildServer.TeamFoundation


Target.initEnvironment()

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

type Branch =
    | Dev
    | Master

type FeedVersion =
    | Alpha
    | PreRelease
    | NuGet

// --------------------------------------------------------------------------------------
// Build variables
// --------------------------------------------------------------------------------------

let host = match Environment.hasEnvironVar("BUILD_BUILDID") with
            | true -> AzureDevOps
            | false -> Local

//let feedVersion = match Environment.hasEnvironVar("BUILD_BUILDID") with

let platform =
    if Environment.isMacOS then OSX
    elif Environment.isLinux then Linux
    else Windows

let buildNumber = match host with
                    | Local -> "0.0.1"
                    | AzureDevOps -> Environment.environVarOrDefault "BUILD_BUILDNUMBER" "0.0.1"

let runtimeIds = dict[Windows, "win-x64"; Linux, "linux-x64"; OSX, "osx-x64"]
let runtimeId = runtimeIds.Item(platform);
let configuration = DotNet.BuildConfiguration.Release
let solution = System.IO.Path.GetFullPath(string "../Akkatecture.sln")
let sourceDirectory =  System.IO.Path.GetFullPath(string "../")
let testResults = sourceDirectory @@ "testresults"
let pushesToFeed = match host with 
                    | AzureDevOps -> true
                    | _ -> false

// --------------------------------------------------------------------------------------
// Build Current Working Directory
// --------------------------------------------------------------------------------------
System.Environment.CurrentDirectory <- sourceDirectory

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

    //cleanables |> Seq.iter Trace.log 

)

Target.create "Restore" (fun _ ->
    Trace.log " --- Restoring Solution --- "

    DotNet.restore id solution
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

Target.create "Push" (fun _ ->
    Trace.log " --- Publish Packages --- "

    let glob = sprintf "src/**/bin/%A/*.nupkg" configuration

    let pushables =
        !! glob

    pushables |> Seq.iter Trace.log 
)

Target.create "All" ignore

// --------------------------------------------------------------------------------------
// Build order
// --------------------------------------------------------------------------------------

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "MultiNodeTest"
  =?> ("Push", pushesToFeed)
  ==> "All"

Target.runOrDefault "Build"
