using LibUC3;
using OpenTK.Mathematics;

namespace UC3Tools;

public sealed class TerrainGenTool : ITool
{
    private enum TerrainResolution : int
    {
        Sparse = 64,
        Medium = 128,
        Dense = 256,
        UltraDense = 512
    }

    public string Name => "Terrain File Generator";

    public Control CreateUI()
{
    var layout = new TableLayoutPanel
    {
        Dock = DockStyle.Fill,
        ColumnCount = 3,
        RowCount = 8,
        Padding = new Padding(10),
        AutoSize = true
    };

    layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
    layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
    layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
    
    var txtOut = new TextBox { Dock = DockStyle.Fill };
    var cmbResolution = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    cmbResolution.Items.AddRange(Enum.GetNames(typeof(TerrainResolution)));
    cmbResolution.SelectedIndex = 1;

    var numWidth = new NumericUpDown
    {
        Dock = DockStyle.Fill,
        Minimum = 10,
        Maximum = 10000,
        Value = 50,
        Increment = 5
    };

    var chkUseHeightmap = new CheckBox { Text = "Use Heightmap", AutoSize = true };
    var txtHeightmap = new TextBox { Dock = DockStyle.Fill, Enabled = false };
    var btnBrowseHeightmap = MakeBrowseButton(txtHeightmap, true, "Heightmap (*.png;*.jpg)|*.png;*.jpg");
    btnBrowseHeightmap.Enabled = false;

    var numHeightmapHeight = new NumericUpDown
    {
        Dock = DockStyle.Fill,
        Minimum = 1,
        Maximum = 1000,
        Value = 10,
        Increment = 1,
        Enabled = false
    };

    var lblStatus = new Label { Text = "Idle", AutoSize = true };
    var btnGenerate = new Button { Text = "Generate", Dock = DockStyle.Right };
    
    chkUseHeightmap.CheckedChanged += (_, _) =>
    {
        bool enabled = chkUseHeightmap.Checked;
        txtHeightmap.Enabled = enabled;
        btnBrowseHeightmap.Enabled = enabled;
        numHeightmapHeight.Enabled = enabled;
    };
    
    layout.Controls.Add(new Label { Text = "Output File:" }, 0, 0);
    layout.Controls.Add(txtOut, 1, 0);
    layout.Controls.Add(MakeBrowseButton(txtOut, false, "UC3 World (*.uc3world)|*.uc3world"), 2, 0);

    layout.Controls.Add(new Label { Text = "Resolution:" }, 0, 1);
    layout.Controls.Add(cmbResolution, 1, 1);

    layout.Controls.Add(new Label { Text = "Terrain Width:" }, 0, 2);
    layout.Controls.Add(numWidth, 1, 2);
    layout.Controls.Add(new Label { Text = "(meters)" }, 2, 2);

    layout.Controls.Add(chkUseHeightmap, 0, 3);

    layout.Controls.Add(new Label { Text = "Heightmap File:" }, 0, 4);
    layout.Controls.Add(txtHeightmap, 1, 4);
    layout.Controls.Add(btnBrowseHeightmap, 2, 4);

    layout.Controls.Add(new Label { Text = "Max Height (m):" }, 0, 5);
    layout.Controls.Add(numHeightmapHeight, 1, 5);

    layout.Controls.Add(btnGenerate, 2, 6);
    layout.Controls.Add(lblStatus, 1, 7);
    
    
    btnGenerate.Click += (_, _) =>
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtOut.Text))
            {
                MessageBox.Show("Please select an output file", "Output Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkUseHeightmap.Checked && string.IsNullOrWhiteSpace(txtHeightmap.Text))
            {
                MessageBox.Show("Please select a heightmap file", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblStatus.Text = "Generating...";
            lblStatus.ForeColor = Color.Blue;
            btnGenerate.Enabled = false;
            Application.DoEvents();

            var resolution = (TerrainResolution)Enum.Parse(typeof(TerrainResolution), cmbResolution.SelectedItem.ToString());
            int width = (int)numWidth.Value;
            bool useHeightmap = chkUseHeightmap.Checked;
            string heightmapPath = txtHeightmap.Text;
            float heightmapHeight = (float)numHeightmapHeight.Value;

            GenerateTerrain(txtOut.Text, heightmapPath, useHeightmap, heightmapHeight, width, resolution);

            lblStatus.Text = "Done!";
            lblStatus.ForeColor = Color.Green;
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error";
            lblStatus.ForeColor = Color.Red;
            MessageBox.Show($"Terrain generation failed:\n\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnGenerate.Enabled = true;
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

    private static void GenerateTerrain(string path, string heightmapPath, bool useHeightmap, float heightmapHeight,
        int totalWidth, TerrainResolution resolution)
    {
        float halfWidth = 0.5f * totalWidth;
        float vertexSpacing = halfWidth / (int)resolution;

        var world = UC3.CreateBlankWorld();

        world.World.Terrain.ResetTerrain((int)resolution, new Color4(20, 117, 65, 255));
        world.World.Terrain.VertexSpacing = vertexSpacing;
        
        var widthAndHeight = world.World.Terrain.VerticesPerDirection * 2 + 1;
        
        if (useHeightmap && File.Exists(heightmapPath))
        {
            using var bitmap = new Bitmap(heightmapPath);

            int terrainSize = world.World.Terrain.VerticesPerDirection * 2 + 1;

            for (int row = 0; row < terrainSize; row++)
            {
                for (int col = 0; col < terrainSize; col++)
                {
                    int x = col * bitmap.Width / terrainSize;
                    int y = row * bitmap.Height / terrainSize;

                    var pixel = bitmap.GetPixel(x, y);
                    float gray = ((pixel.R + pixel.G + pixel.B) / 3f) / 255f;
                    float height = gray * heightmapHeight;

                    world.World.Terrain.SetHeightAtCoord(col, row, height); // needs to be flipped for some raisin
                }
            }
        }

        File.WriteAllBytes(path, world.SerializeToBytes());
    }
}