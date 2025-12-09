//open System
//open DictionaryCore
//open JsonStorage

//let printMenu () =
//    printfn ""
//    printfn "==== Dictionary App ===="
//    printfn "1) Add word"
//    printfn "2) Update word"
//    printfn "3) Delete word"
//    printfn "4) Search exact"
//    printfn "5) Search partial"
//    printfn "6) Save"
//    printfn "7) Load"
//    printfn "8) Exit"
//    printf "Choose: "

//[<EntryPoint>]
//let main _ =
//    let mutable dict = Map.empty
//    let filePath = "dictionary.json"

//    let rec loop () =
//        printMenu ()
//        match Console.ReadLine() with
//        | "1" ->
//            printf "Word: "
//            let w = Console.ReadLine()
//            printf "Definition: "
//            let d = Console.ReadLine()
//            match addWord w d dict with
//            | Ok nd -> dict <- nd; printfn "Added."
//            | Error e -> printfn "Error: %s" e
//            loop()

//        | "2" ->
//            printf "Word: "
//            let w = Console.ReadLine()
//            printf "New Definition: "
//            let d = Console.ReadLine()
//            match updateWord w d dict with
//            | Ok nd -> dict <- nd; printfn "Updated."
//            | Error e -> printfn "Error: %s" e
//            loop()

//        | "3" ->
//            printf "Word: "
//            let w = Console.ReadLine()
//            match deleteWord w dict with
//            | Ok nd -> dict <- nd; printfn "Deleted."
//            | Error e -> printfn "Error: %s" e
//            loop()

//        | "4" ->
//            printf "Search word: "
//            let w = Console.ReadLine()
//            match findExact w dict with
//            | Some d -> printfn "Definition: %s" d
//            | None -> printfn "Not found."
//            loop()

//        | "5" ->
//            printf "Search part: "
//            let p = Console.ReadLine()
//            let results = searchPartial p dict
//            for (w,d) in results do
//                printfn "- %s : %s" w d
//            loop()

//        | "6" ->
//            saveToFile filePath dict
//            printfn "Saved."
//            loop()

//        | "7" ->
//            dict <- loadFromFile filePath
//            printfn "Loaded."
//            loop()

//        | "8" -> printfn "Bye!"
//        | _ -> printfn "Invalid choice"; loop()

//    loop()
//    0


open System
open System.Windows.Forms
open Gui

[<EntryPoint>]
let main args =
    try
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        
        // Set up global exception handler
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        
        Application.ThreadException.Add(fun args ->
            MessageBox.Show(
                sprintf "An unexpected error occurred:\n\n%s\n\nStack trace:\n%s" 
                    args.Exception.Message 
                    args.Exception.StackTrace,
                "Application Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            ) |> ignore
        )
        
        Application.Run(startGui())
        0
    with
    | ex ->
        MessageBox.Show(
            sprintf "Failed to start application:\n\n%s\n\nStack trace:\n%s" 
                ex.Message 
                ex.StackTrace,
            "Startup Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        ) |> ignore
        1
