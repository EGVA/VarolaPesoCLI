using VarolaPesaCli.Domain;
using VarolaPesaCli.Ui;

namespace VarolaPesaCli
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            LoopHandler handler = LoopHandler.Instance;
            handler.Run();
        }
    }
}