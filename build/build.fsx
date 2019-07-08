#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.create "Clean" (fun _ ->
    Trace.log " --- Cleaning stuff --- "
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs 

    !! "src/**/bin"
    ++ "src/**/obj" |> Seq.iter Trace.trace
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
