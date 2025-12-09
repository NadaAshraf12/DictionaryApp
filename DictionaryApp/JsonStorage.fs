module JsonStorage

open System
open System.IO
open Newtonsoft.Json
open DictionaryCore

type Entry = {
    word : string
    definition : string
}

let saveToFile (path:string) (dict:Dictionary) =
    try
        // التأكد من وجود المجلد
        let dir = Path.GetDirectoryName(path)
        if not (String.IsNullOrWhiteSpace(dir)) && not (Directory.Exists(dir)) then
            Directory.CreateDirectory(dir) |> ignore
        
        let entries =
            dict
            |> Map.toList
            |> List.map (fun (w,d) -> { word = w; definition = d })

        File.WriteAllText(path, JsonConvert.SerializeObject(entries, Formatting.Indented))
        Ok ()
    with
    | :? UnauthorizedAccessException as ex -> 
        Error (sprintf "Access denied: %s" ex.Message)
    | :? DirectoryNotFoundException as ex -> 
        Error (sprintf "Directory not found: %s" ex.Message)
    | :? IOException as ex -> 
        Error (sprintf "IO error: %s" ex.Message)
    | ex -> 
        Error (sprintf "Error saving file: %s" ex.Message)

let loadFromFile (path:string) : Result<Dictionary, string> =
    if not (File.Exists path) then
        Ok Map.empty
    else
        try
            let json = File.ReadAllText path
            if String.IsNullOrWhiteSpace(json) then
                Ok Map.empty
            else
                try
                    let entries = JsonConvert.DeserializeObject<Entry list>(json)
                    if isNull (box entries) then
                        Ok Map.empty
                    else
                        let dict = entries |> List.fold (fun acc e -> acc |> Map.add e.word e.definition) Map.empty
                        Ok dict
                with
                | :? JsonException as ex -> 
                    Error (sprintf "Invalid JSON format: %s" ex.Message)
                | ex -> 
                    Error (sprintf "Error parsing JSON: %s" ex.Message)
        with
        | :? UnauthorizedAccessException as ex -> 
            Error (sprintf "Access denied: %s" ex.Message)
        | :? IOException as ex -> 
            Error (sprintf "IO error: %s" ex.Message)
        | ex -> 
            Error (sprintf "Error loading file: %s" ex.Message)
