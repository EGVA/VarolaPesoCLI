using VarolaPesaCli.Domain;
using VarolaPesaCli.Ui;

namespace VarolaPesaCli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var stopLoop = false;
            var uiInstance = RenderSpectreUi.Instance;
            var uranoInstance = NUranoIoHandler.Instance;
            var notifierInstance = ClassesNotifier.Instance;
            // Setups the terminal layout.
            uiInstance.Setup();
            // Show io ports options, and set portname to nUranoIoHandler
            uiInstance.ShowWelcomeScreen();
            // Try to open port connection
            uranoInstance.OpenPort();

            while (!stopLoop)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Escape)
                        stopLoop = true;
                }
                
                // Request output from scale
                uranoInstance.SendIoRequest();
                Thread.Sleep(100);
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