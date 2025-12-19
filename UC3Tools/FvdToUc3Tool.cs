namespace UC3Tools;

public sealed class FvdToUc3Tool : ITool
{
    public string Name => "FVD CSV → UC3 World";

    public Control CreateUI()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 6, // Increased row count for new controls
            Padding = new Padding(10),
            AutoSize = true
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var txtCsv = new TextBox { Dock = DockStyle.Fill };
        var txtOut = new TextBox { Dock = DockStyle.Fill };
        var trkHeartlining = new TrackBar 
        { 
            Dock = DockStyle.Fill,
            Minimum = 0,
            Maximum = 100,
            Value = 80,
            TickFrequency = 10,
            SmallChange = 5,
            LargeChange = 20
        };
        var lblHeartliningValue = new Label 
        { 
            Text = "0.8",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter
        };
        var lblStatus = new Label { Text = "Idle", AutoSize = true };
        var btnConvert = new Button { Text = "Convert", Dock = DockStyle.Right };

        // Update heartlining value label when trackbar changes
        trkHeartlining.ValueChanged += (_, _) =>
        {
            float value = trkHeartlining.Value / 100f;
            lblHeartliningValue.Text = value.ToString("F2");
        };

        // Add controls to layout
        layout.Controls.Add(new Label { Text = "FVD CSV:" }, 0, 0);
        layout.Controls.Add(txtCsv, 1, 0);
        layout.Controls.Add(MakeBrowseButton(txtCsv, true, "CSV Files (*.csv)|*.csv"), 2, 0);

        layout.Controls.Add(new Label { Text = "UC3 Output:" }, 0, 1);
        layout.Controls.Add(txtOut, 1, 1);
        layout.Controls.Add(MakeBrowseButton(txtOut, false, "UC3 World (*.uc3world)|*.uc3world"), 2, 1);

        layout.Controls.Add(new Label { Text = "Heartlining:" }, 0, 2);
        layout.Controls.Add(trkHeartlining, 1, 2);
        layout.Controls.Add(lblHeartliningValue, 2, 2);

        // Add tooltip for heartlining
        var toolTip = new ToolTip();
        toolTip.SetToolTip(trkHeartlining, "Heartlining (0=no heartlining, 1=one meter heartlining)");
        toolTip.SetToolTip(lblHeartliningValue, "Heartlining");

        layout.Controls.Add(btnConvert, 2, 3);
        layout.Controls.Add(lblStatus, 1, 4);

        // Add an empty row for spacing
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        btnConvert.Click += (_, _) =>
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtCsv.Text))
                {
                    MessageBox.Show("Please select an input CSV file", "Input Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtOut.Text))
                {
                    MessageBox.Show("Please select an output file", "Output Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(txtCsv.Text))
                {
                    MessageBox.Show("Input file does not exist", "File Not Found", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblStatus.Text = "Converting...";
                lblStatus.ForeColor = Color.Blue;
                Application.DoEvents();

                var args = new Converter.FVDConverterArgs
                {
                    Path = txtCsv.Text,
                    Output = txtOut.Text,
                    Heartlining = trkHeartlining.Value / 100f
                };

                Converter.ConvertFVDCsvToUC3(args);

                lblStatus.Text = "Done!";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Conversion failed:\n\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        return layout;
    }

    private static Button MakeBrowseButton(TextBox target, bool open, string filter)
    {
        var btn = new Button { Text = "Browse", Width = 80 };
        btn.Click += (_, _) =>
        {
            FileDialog dlg = open
                ? new OpenFileDialog { Filter = filter }
                : new SaveFileDialog { Filter = filter };

            if (dlg.ShowDialog() == DialogResult.OK)
                target.Text = dlg.FileName;
        };
        return btn;
    }
}

public sealed class Uc2ToUc3Tool : ITool
{
    public string Name => "UC2 → UC3 World";

    public Control CreateUI()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 4,
            Padding = new Padding(10),
            AutoSize = true
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var txtInput = new TextBox { Dock = DockStyle.Fill };
        var txtOutput = new TextBox { Dock = DockStyle.Fill };
        var lblStatus = new Label { Text = "Idle", AutoSize = true };
        var btnConvert = new Button { Text = "Convert", Dock = DockStyle.Right };

        // Add controls to layout
        layout.Controls.Add(new Label { Text = "UC2 File:" }, 0, 0);
        layout.Controls.Add(txtInput, 1, 0);
        layout.Controls.Add(MakeBrowseButton(txtInput, true, "UC2 Files (*.uc2sav)|*.uc2sav"), 2, 0);

        layout.Controls.Add(new Label { Text = "UC3 Output:" }, 0, 1);
        layout.Controls.Add(txtOutput, 1, 1);
        layout.Controls.Add(MakeBrowseButton(txtOutput, false, "UC3 World (*.uc3world)|*.uc3world"), 2, 1);

        layout.Controls.Add(btnConvert, 2, 2);
        layout.Controls.Add(lblStatus, 1, 3);

        // Add an empty row for spacing
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        btnConvert.Click += (_, _) =>
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    MessageBox.Show("Please select an input UC2 file", "Input Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtOutput.Text))
                {
                    MessageBox.Show("Please select an output file", "Output Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(txtInput.Text))
                {
                    MessageBox.Show("Input file does not exist", "File Not Found", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Set default output filename if not specified
                if (string.IsNullOrWhiteSpace(txtOutput.Text) && !string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    txtOutput.Text = Path.ChangeExtension(txtInput.Text, ".uc3world");
                }

                lblStatus.Text = "Converting...";
                lblStatus.ForeColor = Color.Blue;
                Application.DoEvents();

                var args = new UC2ToUC3Converter.UC2ToUC3ConverterArgs
                {
                    Path = txtInput.Text,
                    Output = txtOutput.Text
                };

                UC2ToUC3Converter.Convert(args);

                lblStatus.Text = "Done!";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Conversion failed:\n\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        // Auto-fill output filename when input changes
        txtInput.TextChanged += (_, _) =>
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text) && 
                string.IsNullOrWhiteSpace(txtOutput.Text) &&
                File.Exists(txtInput.Text))
            {
                txtOutput.Text = Path.ChangeExtension(txtInput.Text, ".uc3world");
            }
        };

        return layout;
    }

    private static Button MakeBrowseButton(TextBox target, bool open, string filter)
    {
        var btn = new Button { Text = "Browse", Width = 80 };
        btn.Click += (_, _) =>
        {
            FileDialog dlg = open
                ? new OpenFileDialog { Filter = filter }
                : new SaveFileDialog { Filter = filter };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                target.Text = dlg.FileName;
            }
        };
        return btn;
    }
}