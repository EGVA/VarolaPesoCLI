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
                        .PageSize(10)
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
                        new Markup($"Peso: {weight.WeightValue.ToString("N3")}/kg \n Tara: {weight.Tara.ToString("N3")}/kg\nValor: {weight.Price.ToString("N2")}R$/Kg\nTotal: {weight.Total.ToString("N2")}R$\n{NUranoIoHandler.Instance.inSequenceSameWeightQtt} / {NUranoIoHandler.sequenceToPrint}"),
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
        // var grid = new Grid();
        // grid.AddColumn();

        // grid.AddRow($"Exception: {e.Message}");
        // grid.AddRow($"Stack Trace: {e.StackTrace}");
        // grid.AddRow($"Source: {e.Source}");
        // grid.AddEmptyRow();
        // grid.AddEmptyRow();
        // grid.AddRow($"Aperte Qualquer tecla para reinciar");

        // Create a list of Items
        var rows = new List<Text>(){
                new Text($"Exception: {e.Message}"),
                new Text($"Stack Trace: {e.StackTrace}"),
                new Text($"Source: {e.Source}")
            };

        // Render each item in list on separate line
        AnsiConsole.Write(new Rows(rows));


        // GuiLayout["Root"].Update(
        //     new Panel(Align.Center(grid).VerticalAlignment(VerticalAlignment.Middle)
        //     ).Expand());
        // AnsiConsole.Write(GuiLayout);
        Console.ReadKey();
    }
}