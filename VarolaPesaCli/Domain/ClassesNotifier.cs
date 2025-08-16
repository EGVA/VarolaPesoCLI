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
        var printer = PrintHandler.ConnectNetworkPrinter("192.168.3.154", 9100, "Caixa");
        RenderSpectreUi.Instance.UpdateWeightValues(weight);
        if (weight.WeightValue > 0 && !NUranoIoHandler.Instance.alreadyPrinted)
            PrintHandler.PrintWeight(weight, printer);
    }
}
