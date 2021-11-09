#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.Paket
nuget Fake.BuildServer.GitHubActions
"

#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet

let version = Environment.environVarOrDefault "VERSION" "0.0.1"
let nugetKey = Environment.environVarOrDefault "NUGET_API_KEY" ""

let v0 = "src/AuthZedClient.V0/AuthZedClient.V0.csproj"

let v1 = "src/AuthZedClient.V1/AuthZedClient.V1.csproj"

let sln = "authzed-dotnet.sln"
let packageDir = "publish/"

Target.create "Clean" ( fun _ ->
    Trace.log " --- clean stuff ---"
    DotNet.exec id "clean" $"{sln}" |> ignore
    Shell.cleanDir packageDir
    )

Target.create "Restore" (fun _ ->
    Trace.log "--- restoring paket ---"
    Paket.restore (fun opt -> { opt with ToolType = ToolType.CreateLocalTool () } )
                         )

Target.create "BuildV1" (fun _ ->
    Trace.log "--- build ---"
    DotNet.build (fun opt -> { opt with
                                   Configuration = DotNet.BuildConfiguration.Release
                               }) v1
                       )

Target.create "BuildV0" (fun _ ->
    Trace.log "--- build ---"
    DotNet.build (fun opt -> { opt with
                                   Configuration = DotNet.BuildConfiguration.Release
                               }) v0
                         )

let makePackOptions template (opt : Paket.PaketPackParams) =
    { opt with
        ToolType = ToolType.CreateLocalTool ()
        TemplateFile = template
        Version = version
        OutputPath = packageDir
        LockDependencies = true
    }    

Target.create "PackV0" (fun _ ->
    Trace.log "--- publishing V0 ---"                                        
    Paket.pack (makePackOptions "src/AuthZedClient.V0/paket.template")    
    )

Target.create "PackV1" (fun _ ->
    Trace.log "--- publishing V1 ---"                                        
    Paket.pack (makePackOptions "src/AuthZedClient.V1/paket.template")    
    )

let makePushOptions (opt: Paket.PaketPushParams) =
    { opt with
        ToolType = ToolType.CreateLocalTool ()
        ApiKey = nugetKey
        WorkingDir = packageDir
        PublishUrl = "https://api.nuget.org/v3/index.json" }

Target.create "Release" (fun _ ->
        !! $"{packageDir}/*nupkg"
        |> Paket.pushFiles makePushOptions
    )


Target.create "Pack" ignore
Target.create "Build" ignore

"Restore" ==> "BuildV0" ==> "PackV0"
"Restore" ==> "BuildV1" ==> "PackV1"

"BuildV0" ==> "Build"
"BuildV1" ==> "Build"

"PackV0" ==> "Pack"
"PackV1" ==> "Pack"

"Clean" ==> "Pack" ==> "Release" 

Target.runOrDefault "BuildV1"
