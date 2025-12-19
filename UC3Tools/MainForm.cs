namespace UC3Tools;

public sealed class MainForm : Form
{
    private readonly ComboBox _toolSelector;
    private readonly Panel _toolHost;

    private readonly ITool[] _tools =
    {
        new FvdToUc3Tool(),
        new Uc2ToUc3Tool()
    };

    public MainForm()
    {
        Text = "UC3 Tools";
        Width = 400;
        Height = 250;

        _toolSelector = new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        
        _toolSelector.Items.Add("-- select tool --");
        _toolSelector.Items.AddRange(_tools.Select(t => t.Name).ToArray());
        _toolSelector.SelectedIndexChanged += ToolChanged;

        _toolHost = new Panel
        {
            Dock = DockStyle.Fill
        };

        Controls.Add(_toolHost);
        Controls.Add(_toolSelector);

        _toolSelector.SelectedIndex = 0;
    }

    private void ToolChanged(object? sender, EventArgs e)
    {
        _toolHost.Controls.Clear();
        
        if (_toolSelector.SelectedIndex <= 0)
            return;
        
        var tool = _tools[_toolSelector.SelectedIndex - 1];
        var ui = tool.CreateUI();

        ui.Dock = DockStyle.Fill;
        _toolHost.Controls.Add(ui);
    }
}

public interface ITool
{
    string Name { get; }
    Control CreateUI();
}