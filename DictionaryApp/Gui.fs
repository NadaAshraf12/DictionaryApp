module Gui

open System
open System.Drawing
open System.Windows.Forms
open DictionaryCore
open JsonStorage

let startGui () =
    let form = new Form(
        Text = "F# Dictionary Application",
        WindowState = FormWindowState.Maximized,
        BackColor = Color.WhiteSmoke,
        Padding = new Padding(0)
    )

    // COMMON FONT
    let mainFont = new Font("Segoe UI", 11.0f, FontStyle.Regular)
    let titleFont = new Font("Segoe UI", 14.0f, FontStyle.Bold)
    let labelFont = new Font("Segoe UI", 10.0f, FontStyle.Regular)

    // TITLE BAR
    let titlePanel = new Panel(
        Dock = DockStyle.Top, 
        Height = 70,
        BackColor = Color.FromArgb(41, 128, 185)
    )
    
    let titleLabel = new Label(
        Text = "F# Dictionary Manager",
        Font = titleFont,
        ForeColor = Color.White,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter
    )
    
    titlePanel.Controls.Add(titleLabel)

    // MAIN INPUT PANEL - سوف نستخدم FlowLayoutPanel لترتيب أفضل
    let mainPanel = new TableLayoutPanel(
        Dock = DockStyle.Top,
        Height = 160,
        Padding = new Padding(30),
        BackColor = Color.White,
        RowCount = 2,
        ColumnCount = 1
    )
    
    mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f)) |> ignore
    mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f)) |> ignore

    // WORD ROW - أولاً
    let wordRowPanel = new Panel(
        Height = 60,
        Dock = DockStyle.Fill
    )
    
    let lblWord = new Label(
        Text = "Word:",
        Font = labelFont,
        Width = 100,
        Location = new Point(0, 20)
    )
    
    let txtWord = new TextBox(
        Font = mainFont,
        Width = 350,
        Height = 35,
        Location = new Point(110, 15)
    )

    wordRowPanel.Controls.AddRange [| lblWord :> Control; txtWord :> Control |]
    mainPanel.Controls.Add(wordRowPanel, 0, 0)

    // DEFINITION ROW - ثانياً
    let defRowPanel = new Panel(
        Height = 60,
        Dock = DockStyle.Fill
    )
    
    let lblDef = new Label(
        Text = "Definition:",
        Font = labelFont,
        Width = 100,
        Location = new Point(0, 20)
    )
    
    let txtDef = new TextBox(
        Font = mainFont,
        Width = 550,
        Height = 35,
        Location = new Point(110, 15),
        Multiline = true
    )

    defRowPanel.Controls.AddRange [| lblDef :> Control; txtDef :> Control |]
    mainPanel.Controls.Add(defRowPanel, 0, 1)

    // BUTTON PANEL
    let btnPanel = new FlowLayoutPanel(
        Dock = DockStyle.Top,
        Height = 70,
        Padding = new Padding(30, 10, 30, 10),
        BackColor = Color.FromArgb(240, 240, 240),
        FlowDirection = FlowDirection.LeftToRight
    )

    // دالة لإنشاء أزرار مع Emoji
    let makeButton (text: string) color =
        new Button(
            Text = text,
            Width = 140,
            Height = 40,
            Font = new Font("Segoe UI Symbol", 11.0f),  // خط يدعم الرموز
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(5),
            Cursor = Cursors.Hand
        )

    let btnAdd = makeButton "➕ Add Word" (Color.FromArgb(46, 204, 113))
    let btnUpdate = makeButton "✏️ Update" (Color.FromArgb(52, 152, 219))
    let btnDelete = makeButton "X Delete" (Color.FromArgb(231, 76, 60))
    let btnSearch = makeButton "🔍 Search" (Color.FromArgb(155, 89, 182))
    let btnSave = makeButton "💾 Save" (Color.FromArgb(241, 196, 15))
    let btnLoad = makeButton "📂 Load" (Color.FromArgb(52, 73, 94)) 

    btnPanel.Controls.AddRange [|
        btnAdd :> Control; btnUpdate :> Control; btnDelete :> Control; 
        btnSearch :> Control; btnSave :> Control; btnLoad :> Control
    |]

    // STATUS BAR
    let statusPanel = new Panel(
        Dock = DockStyle.Bottom,
        Height = 30,
        BackColor = Color.FromArgb(52, 73, 94)
    )
    
    let statusLabel = new Label(
        Text = "Ready",
        Font = new Font("Segoe UI", 9.0f),
        ForeColor = Color.White,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(10, 0, 0, 0)
    )
    
    statusPanel.Controls.Add(statusLabel)

    // LISTBOX FOR DICTIONARY ENTRIES
    let listPanel = new Panel(
        Dock = DockStyle.Fill,
        Padding = new Padding(30)
    )
    
    let listBox = new ListBox(
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI", 11.0f),
        BackColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle
    )

    listPanel.Controls.Add(listBox)

    // Add all controls to form بالترتيب الصحيح
    form.Controls.AddRange [|
        listPanel :> Control      // يأتي أولاً لكن مع Dock Fill
        btnPanel :> Control       // يأتي ثانياً فوق ListBox
        mainPanel :> Control      // يأتي ثالثاً فوق Buttons
        titlePanel :> Control     // يأتي رابعاً في الأعلى
        statusPanel :> Control    // يأتي أخيراً في الأسفل
    |]

    // LOGIC SECTION
    let mutable dict = Map.empty
    let filePath = @"D:\Backend\DictionaryApp\DictionaryApp\dictionary.json"

    let updateStatus message =
        statusLabel.Text <- message
        statusLabel.ForeColor <- Color.White

    let refreshList () =
        listBox.Items.Clear()
        dict 
        |> Map.toList
        |> List.sortBy fst
        |> List.iter (fun (word, definition) -> 
            listBox.Items.Add(sprintf "📖 %s : %s" word definition) |> ignore
        )
        updateStatus (sprintf "Dictionary contains %d words" (Map.count dict))

    let showSuccess msg =
        updateStatus msg
        MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore

    let showError msg =
        updateStatus msg
        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

    let showInfo msg =
        updateStatus msg
        MessageBox.Show(msg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore

    // Event Handlers
    btnAdd.Click.Add(fun _ ->
        let word = txtWord.Text.Trim()
        let definition = txtDef.Text.Trim()
        
        if String.IsNullOrWhiteSpace(word) then
            showError "Please enter a word"
        elif String.IsNullOrWhiteSpace(definition) then
            showError "Please enter a definition"
        else
            match addWord word definition dict with
            | Ok nd -> 
                dict <- nd
                refreshList()
                txtWord.Clear()
                txtDef.Clear()
                txtWord.Focus() |> ignore
                showSuccess (sprintf "Word '%s' added successfully!" word)
            | Error e -> showError e
    )

    btnUpdate.Click.Add(fun _ ->
        let word = txtWord.Text.Trim()
        let definition = txtDef.Text.Trim()
        
        if String.IsNullOrWhiteSpace(word) then
            showError "Please enter a word to update"
        elif String.IsNullOrWhiteSpace(definition) then
            showError "Please enter a new definition"
        else
            match updateWord word definition dict with
            | Ok nd -> 
                dict <- nd
                refreshList()
                showSuccess (sprintf "Word '%s' updated successfully!" word)
            | Error e -> showError e
    )

    btnDelete.Click.Add(fun _ ->
        let word = txtWord.Text.Trim()
        if String.IsNullOrWhiteSpace(word) then
            showError "Please enter a word to delete"
        else
            match deleteWord word dict with
            | Ok nd -> 
                dict <- nd
                refreshList()
                txtWord.Clear()
                txtDef.Clear()
                showSuccess (sprintf "Word '%s' deleted successfully!" word)
            | Error e -> showError e
    )

    btnSearch.Click.Add(fun _ ->
        let wordToSearch = txtWord.Text.Trim()
        if String.IsNullOrWhiteSpace(wordToSearch) then
            showError "Please enter a word to search"
        else
            match findExact wordToSearch dict with
            | Some d -> 
                txtDef.Text <- d
                updateStatus (sprintf "Found: %s" wordToSearch)
                // البحث عن العنصر في ال ListBox وتحديده
                let mutable foundIndex = -1
                for i = 0 to listBox.Items.Count - 1 do
                    if foundIndex = -1 then
                        let item = listBox.Items.[i].ToString()
                        if item.Contains(sprintf "📖 %s : " wordToSearch) then
                            foundIndex <- i
                if foundIndex <> -1 then
                    listBox.SelectedIndex <- foundIndex
            | None -> showInfo "Word not found in dictionary"
    )

    btnSave.Click.Add(fun _ ->
        try
            saveToFile filePath dict
            showSuccess (sprintf "Dictionary saved to '%s' successfully!" filePath)
        with
        | ex -> showError (sprintf "Error saving file: %s" ex.Message)
    )

    btnLoad.Click.Add(fun _ ->
        try
            dict <- loadFromFile filePath
            refreshList()
            showSuccess (sprintf "Dictionary loaded from '%s' successfully!" filePath)
        with
        | ex -> showError (sprintf "Error loading file: %s" ex.Message)
    )

    // Keyboard shortcuts
    form.KeyPreview <- true
    form.KeyDown.Add(fun e ->
        if e.Control then
            match e.KeyCode with
            | Keys.S -> btnSave.PerformClick()
            | Keys.L -> btnLoad.PerformClick()
            | Keys.A -> txtWord.Focus() |> ignore
            | _ -> ()
        else
            match e.KeyCode with
            | Keys.Enter -> 
                if txtWord.Focused || txtDef.Focused then
                    btnAdd.PerformClick()
            | Keys.F5 -> btnSave.PerformClick()
            | Keys.F9 -> btnLoad.PerformClick()
            | Keys.Delete -> btnDelete.PerformClick()
            | _ -> ()
    )

    // ListBox selection event
    listBox.SelectedIndexChanged.Add(fun _ ->
        if listBox.SelectedIndex <> -1 then
            let selected = listBox.SelectedItem.ToString()
            if selected.StartsWith("📖 ") then
                let withoutEmoji = selected.Substring(2)
                let parts = withoutEmoji.Split([|" : "|], StringSplitOptions.None)
                if parts.Length >= 2 then
                    txtWord.Text <- parts.[0]
                    txtDef.Text <- parts.[1]
    )

    // إضافة بيانات أولية للاختبار
    dict <- 
        Map.empty
        |> Map.add "Apple" "A sweet red fruit"
        |> Map.add "Book" "A collection of pages"
        |> Map.add "Computer" "An electronic device"
        |> Map.add "Dictionary" "A book that lists words"
    
    // إعدادات أولية
    txtWord.Focus() |> ignore
    refreshList()

    form