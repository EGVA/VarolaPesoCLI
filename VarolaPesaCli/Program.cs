using ESCPOS_NET;
using VarolaPesaCli.Domain;
using VarolaPesaCli.Models;
using VarolaPesaCli.Ui;

namespace VarolaPesaCli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(PrintHandler.BarcodeNumber(new Weight(0.5m, 0, 0, 0)));
            Console.ReadKey();
            while (true)
            {
                Console.Clear();
                UranoIoHandler ioHandler = new UranoIoHandler();
                string portName = RenderUtils.PortOptions(ioHandler);

                ioHandler.OpenPort(portName);
                ioHandler.RequestScale();
            }
        }
    }
}