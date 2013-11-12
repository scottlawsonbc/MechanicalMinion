namespace MechanicalMinion

[<AutoOpen>]
module Retrieval =
    
    open System
    open System.IO

    /// Returns all files in a directory
    let GetAllFiles (directory:string) = 
        if not (Directory.Exists directory) then 
            failwith ("No such directory \"" + directory + "\" exists")
        (new DirectoryInfo(directory)).GetFiles()

    /// Returns the FileInfo of the first file sorted by the given comparison
    let GetFirstFileByComparison (directory:string) (comparison) = 
        let files = GetAllFiles directory
        let sortedFiles = Array.sortBy(fun (f:FileInfo) -> comparison f) files
        match sortedFiles.Length with
        | 0 -> None
        | _ -> Some sortedFiles.[0]

    /// Returns the FileInfo of all files sorted by the given comparison
    let GetSortedFiles (directory:string) (comparison) = 
        let sortedFiles =
            GetAllFiles directory
            |> Array.sortBy( fun (f:FileInfo) -> comparison f)
        match sortedFiles.Length with
        | 0 -> None
        | _ -> Some sortedFiles

    /// Returns the FileInfo of the most recently modified file
    let GetLastModifiedFile (directory:string) : FileInfo option = 
        GetFirstFileByComparison directory (fun (f:FileInfo) -> f.LastWriteTime)

    /// Returns all files in a directory of a specified type
    let GetAllFilesOfType (directory:string) (fileExtension:string) = 
        Array.FindAll(GetAllFiles directory, fun f -> f.Extension.ToUpper() = fileExtension.ToUpper()) 

[<AutoOpen>]
module Actions = 

    open System
    open System.IO
    open SharpCompress.Reader

    /// Determine whether the specified file is locked
    let IsFileLocked (file:string) = 
        try 
            use stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            stream.Close()
            true
        with 
            (ex:IOException) -> false

    /// Moves the specified file. Can also be used to rename files
    let Move (source:string) (destination:string) = File.Move(source, destination)

    /// Deletes the specified file
    let Delete (file:string) = File.Delete file

    /// Extracts the file to the specified directory, creating if it does not exist
    let Extract (file:string) (directory:string) (overwrite:bool) = 
        let option = match overwrite with
                     | true -> SharpCompress.Common.ExtractOptions.Overwrite
                     | false -> SharpCompress.Common.ExtractOptions.None
        if not (Directory.Exists directory) then Directory.CreateDirectory directory |> ignore
        use stream = File.OpenRead file
        use reader = ReaderFactory.Open stream
        reader.WriteAllToDirectory(directory, option)
    
    /// Opens a specified file. Does not check to see if file exists or is locked
    let Open (file:string) = 
        System.Diagnostics.Process.Start file

    /// Defines an extract extension method for the FileInfo type
    type FileInfo with
        member x.ExtractTo(directory:string) (overwrite:bool) = 
            Extract x.FullName directory overwrite

//module Monitoring = 
//    
//    open System
//    open System.IO
//    
//    type Watcher() = 
//        let mutable watcher = new FileSystemWatcher()
//        //let mutable action 
//        
//        member x.Watch path filter = 
//            watcher <- new FileSystemWatcher(path, filter)
//            watcher.EnableRaisingEvents <- true
//            watcher.NotifyFilter <- NotifyFilters.LastWrite
//            watcher.Changed += (fun x y -> ())
//

       

        
               
