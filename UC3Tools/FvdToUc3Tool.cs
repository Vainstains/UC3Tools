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
            RowCount = 4,
            Padding = new Padding(10),
            AutoSize = true
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var txtCsv = new TextBox { Dock = DockStyle.Fill };
        var txtOut = new TextBox { Dock = DockStyle.Fill };
        var lblStatus = new Label { Text = "Idle", AutoSize = true };
        var btnConvert = new Button { Text = "Convert", Dock = DockStyle.Right };

        layout.Controls.Add(new Label { Text = "FVD CSV:" }, 0, 0);
        layout.Controls.Add(txtCsv, 1, 0);
        layout.Controls.Add(MakeBrowseButton(txtCsv, true), 2, 0);

        layout.Controls.Add(new Label { Text = "Output:" }, 0, 1);
        layout.Controls.Add(txtOut, 1, 1);
        layout.Controls.Add(MakeBrowseButton(txtOut, false), 2, 1);

        layout.Controls.Add(btnConvert, 2, 2);
        layout.Controls.Add(lblStatus, 1, 3);

        btnConvert.Click += (_, _) =>
        {
            try
            {
                lblStatus.Text = "Converting...";
                Application.DoEvents();

                Converter.ConvertFVDCsvToUC3(txtCsv.Text, txtOut.Text);

                lblStatus.Text = "Done!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                MessageBox.Show(ex.ToString(), "Conversion failed");
            }
        };

        return layout;
    }

    private static Button MakeBrowseButton(TextBox target, bool open)
    {
        var btn = new Button { Text = "Browse" };
        btn.Click += (_, _) =>
        {
            FileDialog dlg = open
                ? new OpenFileDialog { Filter = "CSV Files (*.csv)|*.csv" }
                : new SaveFileDialog { Filter = "UC3 World (*.uc3world)|*.uc3world" };

            if (dlg.ShowDialog() == DialogResult.OK)
                target.Text = dlg.FileName;
        };
        return btn;
    }
}