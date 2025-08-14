using Spectre.Console;
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

    public void ShowWelcomeScreen()
    {
        AnsiConsole.Clear();
        Random random = new Random();
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

    public string ShowPrinterOptions()
    {
        while (true)
        {
            var ports = nUranoIoHandler.GetAllPorts();
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
                nUranoIoHandler.Instance.serialPortName = selectedPort;
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
}