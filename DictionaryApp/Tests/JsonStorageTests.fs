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
            match saveToFile tempFilePath dict with
            | Ok () ->
                File.Exists(tempFilePath) |> should be True
                let content = File.ReadAllText(tempFilePath)
                content |> should not' (be EmptyString)
                
                let entries = JsonConvert.DeserializeObject<JsonStorage.Entry list>(content)
                entries |> List.length |> should equal 3
            | Error msg -> Assert.Fail(sprintf "Should have succeeded but got error: %s" msg)
        finally
            cleanup()

    [<Fact>]
    let ``loadFromFile should load empty dictionary for non-existent file``() =
        let nonExistentPath = @"C:\nonexistent\file.json"
        let result = loadFromFile nonExistentPath
        match result with
        | Ok dict -> Map.isEmpty dict |> should be True
        | Error msg -> Assert.Fail(sprintf "Should have succeeded but got error: %s" msg)

    [<Fact>]
    let ``loadFromFile should load empty dictionary for empty file``() =
        try
            File.WriteAllText(tempFilePath, "")
            let result = loadFromFile tempFilePath
            match result with
            | Ok dict -> Map.isEmpty dict |> should be True
            | Error msg -> Assert.Fail(sprintf "Should have succeeded but got error: %s" msg)
        finally
            cleanup()

    [<Fact>]
    let ``loadFromFile should load dictionary from valid file``() =
        try
            let dict = sampleDict
            match saveToFile tempFilePath dict with
            | Ok () ->
                match loadFromFile tempFilePath with
                | Ok loadedDict ->
                    Map.count loadedDict |> should equal (Map.count dict)
                    Map.containsKey "apple" loadedDict |> should be True
                    Map.find "apple" loadedDict |> should equal "a fruit"
                | Error msg -> Assert.Fail(sprintf "Should have loaded but got error: %s" msg)
            | Error msg -> Assert.Fail(sprintf "Should have saved but got error: %s" msg)
        finally
            cleanup()

    [<Fact>]
    let ``saveToFile and loadFromFile should be symmetrical``() =
        try
            let originalDict = sampleDict
            match saveToFile tempFilePath originalDict with
            | Ok () ->
                match loadFromFile tempFilePath with
                | Ok loadedDict ->
                    Map.toList loadedDict 
                    |> List.sortBy fst 
                    |> should equal (Map.toList originalDict |> List.sortBy fst)
                | Error msg -> Assert.Fail(sprintf "Should have loaded but got error: %s" msg)
            | Error msg -> Assert.Fail(sprintf "Should have saved but got error: %s" msg)
        finally
            cleanup()