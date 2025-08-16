using VarolaPesaCli.Domain;
using VarolaPesaCli.Ui;

namespace VarolaPesaCli
{

    public static class Program
    {
        const int timeToReadScale = 1000;

        public static void Main(string[] args)
        {
            bool stopLoop = false;
            var uiInstance = RenderSpectreUi.Instance;
            var uranoInstance = NUranoIoHandler.Instance;
            var notifierInstance = ClassesNotifier.Instance;
            while (true)
            {
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

                        uranoInstance.ClosePort();
                        break;
                    }

                    // Request output from scale
                    try
                    {
                        uranoInstance.SendIoRequest();
                    }
                    catch (Exception e)
                    {
                        uranoInstance.ClosePort();
                        stopLoop = true;
                    }
                    Thread.Sleep(timeToReadScale);
                }
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