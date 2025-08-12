using VarolaPesaCli.Ui;

namespace VarolaPesaCli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            UranoIoHandler ioHandler = new UranoIoHandler();
            string portName = RenderUtils.PortOptions(ioHandler);
            ioHandler.OpenPort(portName);
            ioHandler.RequestScale();
        }
    }
} 