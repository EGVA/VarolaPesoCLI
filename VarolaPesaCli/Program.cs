using ESCPOS_NET;
using VarolaPesaCli.Domain;
using VarolaPesaCli.Models;
using VarolaPesaCli.Ui;
using Spectre.Console;
using Spectre.Console.Cli;

namespace VarolaPesaCli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var uiInstance = RenderSpectreUi.Instance;
            var uranoInstance = NUranoIoHandler.Instance;
            var notifierInstance = ClassesNotifier.Instance;
            // Setups the terminal layout.
            uiInstance.Setup();
            // Show io ports options, and set portname to nUranoIoHandler
            uiInstance.ShowWelcomeScreen();
            // Try to open port connection
            uranoInstance.OpenPort();

            while (true)
            {
                // Request output from scale
                uranoInstance.SendIoRequest();
            }
            
            // Console.WriteLine(PrintHandler.BarcodeNumber(new Weight(0.5m, 0, 0, 0)));
            // Console.ReadKey();
            // while (true)
            // {
            //     Console.Clear();
            //     UranoIoHandler ioHandler = new UranoIoHandler();
            //     string portName = RenderUtils.PortOptions(ioHandler);
            //
            //     ioHandler.OpenPort(portName);
            //     ioHandler.RequestScale();
            // }
        }
    }
}