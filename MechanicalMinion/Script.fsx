// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r @"C:\Users\Scott\documents\visual studio 2013\Projects\MechanicalMinion\packages\sharpcompress.0.10.1.3\lib\net40\SharpCompress.dll"
#load "MechanicalMinion.fs"


open System
open System.IO
open MechanicalMinion

let path = @"C:\Users\Scott\Downloads\"
let files = Retrieval.GetAllFilesOfType path ".docx"

let target = Path.Combine(path, "Word Documents")
if not(target |> Directory.Exists) then Directory.CreateDirectory(target) |> ignore    

for f in files do
    Console.Write f.FullName

for f in files do
    f.MoveTo(Path.Combine(target, f.Name))
