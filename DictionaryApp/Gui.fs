module Gui

open System
open System.Windows.Forms
open DictionaryCore
open JsonStorage

let startGui () =
    let form = new Form(Text = "F# Dictionary", Width = 500, Height = 400)

    let lblWord = new Label(Text = "Word:", Top = 20, Left = 20)
    let txtWord = new TextBox(Top = 40, Left = 20, Width = 200)

    let lblDef = new Label(Text = "Definition:", Top = 80, Left = 20)
    let txtDef = new TextBox(Top = 100, Left = 20, Width = 300)

    let listBox = new ListBox(Top = 180, Left = 20, Width = 440, Height = 150)

    let btnAdd = new Button(Text = "Add", Top = 140, Left = 20)
    let btnUpdate = new Button(Text = "Update", Top = 140, Left = 80)
    let btnDelete = new Button(Text = "Delete", Top = 140, Left = 160)
    let btnSearch = new Button(Text = "Search", Top = 140, Left = 240)

    let btnSave = new Button(Text = "Save", Top = 140, Left = 320)
    let btnLoad = new Button(Text = "Load", Top = 140, Left = 380)

    form.Controls.Add(lblWord)
    form.Controls.Add(txtWord)
    form.Controls.Add(lblDef)
    form.Controls.Add(txtDef)
    form.Controls.Add(listBox)
    form.Controls.Add(btnAdd)
    form.Controls.Add(btnUpdate)
    form.Controls.Add(btnDelete)
    form.Controls.Add(btnSearch)
    form.Controls.Add(btnSave)
    form.Controls.Add(btnLoad)

    let mutable dict = Map.empty
    let filePath = @"D:\Backend\DictionaryApp\DictionaryApp\dictionary.json"

    let refreshList () =
        listBox.Items.Clear()
        dict |> Map.iter (fun w d -> listBox.Items.Add($"{w} : {d}") |> ignore)

    btnAdd.Click.Add(fun _ ->
        match addWord txtWord.Text txtDef.Text dict with
        | Ok nd ->
            dict <- nd
            MessageBox.Show("Added!") |> ignore
            refreshList()
        | Error e -> MessageBox.Show(e) |> ignore
    )

    btnUpdate.Click.Add(fun _ ->
        match updateWord txtWord.Text txtDef.Text dict with
        | Ok nd ->
            dict <- nd
            MessageBox.Show("Updated!") |> ignore
            refreshList()
        | Error e -> MessageBox.Show(e) |> ignore
    )

    btnDelete.Click.Add(fun _ ->
        match deleteWord txtWord.Text dict with
        | Ok nd ->
            dict <- nd
            MessageBox.Show("Deleted!") |> ignore
            refreshList()
        | Error e -> MessageBox.Show(e) |> ignore
    )

    btnSearch.Click.Add(fun _ ->
        match findExact txtWord.Text dict with
        | Some d -> MessageBox.Show(d) |> ignore
        | None -> MessageBox.Show("Not found") |> ignore
    )

    btnSave.Click.Add(fun _ ->
        saveToFile filePath dict
        MessageBox.Show("Saved!") |> ignore
    )

    btnLoad.Click.Add(fun _ ->
        dict <- loadFromFile filePath
        MessageBox.Show("Loaded!") |> ignore
        refreshList()
    )

    form
