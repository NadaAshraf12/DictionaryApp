module DictionaryCore

open System

type Dictionary = Map<string,string>

let normalize (w:string) =
    w.Trim().ToLower()

let addWord word definition (dict:Dictionary) =
    let key = normalize word
    if key = "" then Error "Word cannot be empty"
    elif dict |> Map.containsKey key then Error "Word already exists"
    else Ok (dict |> Map.add key definition)

let updateWord word definition (dict:Dictionary) =
    let key = normalize word
    if dict |> Map.containsKey key then
        Ok (dict |> Map.add key definition)
    else
        Error "Word not found"

let deleteWord word (dict:Dictionary) =
    let key = normalize word
    if dict |> Map.containsKey key then
        Ok (dict |> Map.remove key)
    else
        Error "Word not found"

let findExact word (dict:Dictionary) =
    let key = normalize word
    dict |> Map.tryFind key

let searchPartial (part:string) (dict:Dictionary) =
    let p = normalize part
    dict
    |> Map.toList
    |> List.filter (fun (w,_) -> w.Contains(p))
