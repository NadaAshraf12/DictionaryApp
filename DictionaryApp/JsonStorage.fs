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
    let entries =
        dict
        |> Map.toList
        |> List.map (fun (w,d) -> { word = w; definition = d })

    File.WriteAllText(path, JsonConvert.SerializeObject(entries, Formatting.Indented))

let loadFromFile (path:string) : Dictionary =
    if not (File.Exists path) then
        Map.empty
    else
        try
            let json = File.ReadAllText path
            if String.IsNullOrWhiteSpace(json) then
                Map.empty
            else
                let entries = JsonConvert.DeserializeObject<Entry list>(json)
                if isNull (box entries) then
                    Map.empty
                else
                    entries |> List.fold (fun acc e -> acc |> Map.add e.word e.definition) Map.empty
        with
        | _ -> Map.empty
