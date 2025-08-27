using ESCPOS_NET;
using VarolaPesaCli.Models;
using VarolaPesaCli.Ui;

namespace VarolaPesaCli.Domain;

public class ClassesNotifier
{
    #region Singleton
    private static readonly ClassesNotifier instance = new ClassesNotifier();
    static ClassesNotifier()
    {
    }

    private ClassesNotifier()
    {
    }

    public static ClassesNotifier Instance
    {
        get { return instance; }
    }

    #endregion Singleton

    public void OnScaleOutput()
    {
        Weight weight = NUranoIoHandler.Instance.GetWeight();

        // var printer = PrintHandler.ConnectNetworkPrinter("192.168.3.145", 9100, "Marmita");
        var printer = PrintHandler.ConnectNetworkPrinter("127.0.0.1", 9100, "Debug");

        RenderSpectreUi.Instance.UpdateWeightValues(weight);
        if (NUranoIoHandler.Instance.CanPrint())
        {
            //PrintHandler.PrintUSB(weight);
            PrintHandler.PrintWeight(weight, printer);
        }
    }

    public void OnExceptionInWeightScreen()
    {

    }
}
