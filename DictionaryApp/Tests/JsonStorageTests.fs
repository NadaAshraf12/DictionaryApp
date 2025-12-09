module DictionaryApp.Tests.JsonStorageTests

open Xunit
open FsUnit.Xunit
open System
open System.IO
open Newtonsoft.Json
open DictionaryCore
open JsonStorage

module JsonStorageTests =

    let tempFilePath = Path.GetTempFileName()
    let sampleDict = 
        Map.empty
        |> Map.add "apple" "a fruit"
        |> Map.add "book" "for reading"
        |> Map.add "computer" "electronic device"

    let cleanup () =
        if File.Exists(tempFilePath) then
            File.Delete(tempFilePath)

    [<Fact>]
    let ``saveToFile should save dictionary to file``() =
        try
            let dict = sampleDict
            saveToFile tempFilePath dict
            
            File.Exists(tempFilePath) |> should be True
            let content = File.ReadAllText(tempFilePath)
            content |> should not' (be EmptyString)
            
            let entries = JsonConvert.DeserializeObject<JsonStorage.Entry list>(content)
            entries |> List.length |> should equal 3
        finally
            cleanup()

    [<Fact>]
    let ``loadFromFile should load empty dictionary for non-existent file``() =
        let nonExistentPath = @"C:\nonexistent\file.json"
        let result = loadFromFile nonExistentPath
        Map.isEmpty result |> should be True

    [<Fact>]
    let ``loadFromFile should load empty dictionary for empty file``() =
        try
            File.WriteAllText(tempFilePath, "")
            let result = loadFromFile tempFilePath
            Map.isEmpty result |> should be True
        finally
            cleanup()

    [<Fact>]
    let ``loadFromFile should load dictionary from valid file``() =
        try
            let dict = sampleDict
            saveToFile tempFilePath dict
            let loadedDict = loadFromFile tempFilePath
            
            Map.count loadedDict |> should equal (Map.count dict)
            Map.containsKey "apple" loadedDict |> should be True
            Map.find "apple" loadedDict |> should equal "a fruit"
        finally
            cleanup()

    [<Fact>]
    let ``saveToFile and loadFromFile should be symmetrical``() =
        try
            let originalDict = sampleDict
            saveToFile tempFilePath originalDict
            let loadedDict = loadFromFile tempFilePath
            
            Map.toList loadedDict 
            |> List.sortBy fst 
            |> should equal (Map.toList originalDict |> List.sortBy fst)
        finally
            cleanup()