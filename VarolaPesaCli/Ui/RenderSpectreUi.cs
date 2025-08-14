using Spectre.Console;
using Spectre.Console.Extensions;
using VarolaPesaCli.Domain;
using VarolaPesaCli.Models;

namespace VarolaPesaCli.Ui;

public sealed class RenderSpectreUi
{
    #region Singleton
    private static readonly RenderSpectreUi instance = new RenderSpectreUi();
    static RenderSpectreUi()
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

    public void ShowWelcomeScreen()
    {
        AnsiConsole.Clear();
        GuiLayout["Root"].Update(
            new Panel(
                    Align.Center(
                        new FigletText($"Varola Le Peso"),
                        VerticalAlignment.Middle))
                .Expand());
        AnsiConsole.Write(GuiLayout);
        Thread.Sleep(1000);
        ShowPrinterOptions();
    }

    private string ShowPrinterOptions()
    {
        while (true)
        {
            var ports = NUranoIoHandler.GetAllPorts();
            if (ports.Length == 0)
            {
                AnsiConsole.Clear();
                GuiLayout["Root"].Update(
                    new Panel(
                            Align.Center(
                                new Markup("Nenhuma impressora encontrada. Buscando novamente me 5 segundos\nVerifique o cabos e/ou drivers."),
                                VerticalAlignment.Middle))
                        .Expand());
                AnsiConsole.Write(GuiLayout);
                Thread.Sleep(5000);
            }
            else
            {
                AnsiConsole.Clear();
                
                var selectedPort = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Escolha uma das portas:")
                        .PageSize(ports.Length)
                        .AddChoices(ports));
                NUranoIoHandler.Instance.SerialPortName = selectedPort;
                return selectedPort;
            }
        }
    }
    
    public void UpdateWeightValues(Weight weight)
    {
        AnsiConsole.Clear();
        GuiLayout["Root"].Update(
            new Panel(
                    Align.Center(
                        new Markup($"Peso:{weight.WeightValue}\nTara:{weight.Tara}\nValor:{weight.Price}/kg\nTotal:{weight.Total}"),
                        VerticalAlignment.Middle))
                .Expand());
        AnsiConsole.Write(GuiLayout);
    }
    
    public void HandleWeightUpdate(object sender, EventArgs args)
    {
        if(NUranoIoHandler.Instance.GetWeight() != null)
        {
            UpdateWeightValues(NUranoIoHandler.Instance.GetWeight()!);
        }
    }

    public void ShowException(Exception e)
    {
        var grid = new Grid();
        grid.AddColumn();

        grid.AddRow($"Exception: {e.Message}");
        grid.AddRow($"Stack Trace: {e.StackTrace}");
        grid.AddRow($"Source: {e.Source}");
        
        GuiLayout["Root"].Update(
            new Panel(Align.Center(grid).VerticalAlignment(VerticalAlignment.Middle)
            ).Expand());
        AnsiConsole.Write(GuiLayout);
    }
}