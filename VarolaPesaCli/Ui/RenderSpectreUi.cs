using Spectre.Console;

namespace VarolaPesaCli.Ui;

public sealed class RenderSpectreUi
{
    #region Singleton
    private static readonly RenderSpectreUi instance = new RenderSpectreUi();
    static RenderSpectreUi()
    {
    }

    private RenderSpectreUi()
    {
    }

    public static RenderSpectreUi Instance
    {
        get { return instance; }
    }
    
    #endregion Singleton

    public Layout GuiLayout;
    public void Setup()
    {
        AnsiConsole.Clear();
        
        // Create the layout
        GuiLayout = new Layout("Root");

        // Update the left column
        GuiLayout["Root"].Update(
            new Panel(
                    Align.Center(
                        new Markup("Peso:\nTotal:\nTara:"),
                        VerticalAlignment.Middle))
                .Expand());

        // Render the layout
        AnsiConsole.Write(GuiLayout);
    }

    public void UpdateWeightValues()
    {
        AnsiConsole.Clear();
        Random random = new Random();
        GuiLayout["Root"].Update(
            new Panel(
                    Align.Center(
                        new Markup($"Peso:{random.Next(10)}\nTotal:{random.Next(10)}\nTara:{random.Next(10)}"),
                        VerticalAlignment.Middle))
                .Expand());
        AnsiConsole.Write(GuiLayout);
    }
}