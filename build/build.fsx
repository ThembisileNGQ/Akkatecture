#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let configuration = "Release"
let solution = System.IO.Path.GetFullPath(string "../Akkatecture.sln")
System.Environment.CurrentDirectory <- System.IO.Path.GetFullPath(string "../")

Target.create "Clean" (fun _ ->
    Trace.log " --- Cleaning stuff --- "
    let cleanables = 
        !! "../src/**/bin"
        ++ "../src/**/obj"

    cleanables |> Shell.cleanDirs 

    cleanables |> Seq.iter Trace.trace
    
    Trace.trace System.Environment.CurrentDirectory
    Trace.trace solution
)

Target.create "Restore" (fun _ ->
    Trace.log " --- Restoring stuff --- "
)

Target.create "Build" (fun _ ->
    Trace.log " --- Building stuff --- "
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "Test" (fun _ ->
    Trace.log " --- Testing stuff --- "
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "MultiNodeTest" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "All" ignore

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "MultiNodeTest"
  ==> "All"

Target.runOrDefault "All"
