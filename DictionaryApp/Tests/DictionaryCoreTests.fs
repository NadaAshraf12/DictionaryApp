module DictionaryApp.Tests.DictionaryCoreTests

open Xunit
open FsUnit.Xunit
open DictionaryCore

module DictionaryCoreTests =

    let sampleDict = 
        Map.empty
        |> Map.add "apple" "a fruit"
        |> Map.add "book" "for reading"
        |> Map.add "computer" "electronic device"
        |> Map.add "abacus" "counting tool"

    [<Fact>]
    let ``normalize should convert to lowercase and trim``() =
        normalize "  APPLE  " |> should equal "apple"
        normalize "Hello World" |> should equal "hello world"
        normalize "  TEST  " |> should equal "test"
        normalize "" |> should equal ""
        normalize "  " |> should equal ""

    [<Fact>]
    let ``addWord should add new word successfully``() =
        let dict = Map.empty
        let result = addWord "test" "a trial" dict
        
        match result with
        | Ok newDict ->
            Map.containsKey "test" newDict |> should be True
            Map.find "test" newDict |> should equal "a trial"
        | Error _ -> Assert.Fail("Should have succeeded")

    [<Fact>]
    let ``addWord should return error for empty word``() =
        let dict = Map.empty
        let result = addWord "  " "definition" dict
        
        match result with
        | Ok _ -> Assert.Fail("Should have failed")
        | Error msg -> msg |> should equal "Word cannot be empty"

    [<Fact>]
    let ``addWord should return error for duplicate word``() =
        let dict = Map.empty |> Map.add "apple" "fruit"
        let result = addWord "APPLE" "red fruit" dict
        
        match result with
        | Ok _ -> Assert.Fail("Should have failed")
        | Error msg -> msg |> should equal "Word already exists"

    [<Fact>]
    let ``updateWord should update existing word``() =
        let dict = sampleDict
        let result = updateWord "apple" "delicious fruit" dict
        
        match result with
        | Ok newDict ->
            Map.containsKey "apple" newDict |> should be True
            Map.find "apple" newDict |> should equal "delicious fruit"
        | Error _ -> Assert.Fail("Should have succeeded")

    [<Fact>]
    let ``updateWord should return error for non-existent word``() =
        let dict = sampleDict
        let result = updateWord "nonexistent" "definition" dict
        
        match result with
        | Ok _ -> Assert.Fail("Should have failed")
        | Error msg -> msg |> should equal "Word not found"

    [<Fact>]
    let ``deleteWord should delete existing word``() =
        let dict = sampleDict
        let result = deleteWord "apple" dict
        
        match result with
        | Ok newDict ->
            Map.containsKey "apple" newDict |> should be False
            Map.count newDict |> should equal (Map.count dict - 1)
        | Error _ -> Assert.Fail("Should have succeeded")

    [<Fact>]
    let ``deleteWord should return error for non-existent word``() =
        let dict = sampleDict
        let result = deleteWord "orange" dict
        
        match result with
        | Ok _ -> Assert.Fail("Should have failed")
        | Error msg -> msg |> should equal "Word not found"

    [<Fact>]
    let ``findExact should find existing word``() =
        let dict = sampleDict
        findExact "apple" dict |> should equal (Some "a fruit")
        findExact "APPLE" dict |> should equal (Some "a fruit")
        findExact "  apple  " dict |> should equal (Some "a fruit")

    [<Fact>]
    let ``findExact should return None for non-existent word``() =
        let dict = sampleDict
        findExact "orange" dict |> should equal None
        findExact "" dict |> should equal None

    [<Fact>]
    let ``searchPartial should find words containing substring``() =
        let dict = sampleDict
        let results = searchPartial "app" dict
        
        results |> List.length |> should equal 1
        results |> List.exists (fun (w,_) -> w = "apple") |> should be True

    [<Fact>]
    let ``searchPartial should be case insensitive``() =
        let dict = sampleDict
        let results = searchPartial "APP" dict
        
        results |> List.length |> should equal 1
        results |> List.exists (fun (w,_) -> w = "apple") |> should be True

    [<Fact>]
    let ``searchPartial should return empty list for no matches``() =
        let dict = sampleDict
        let results = searchPartial "xyz" dict
        
        results |> List.isEmpty |> should be True