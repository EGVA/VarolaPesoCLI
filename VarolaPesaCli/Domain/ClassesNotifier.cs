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
        RenderSpectreUi.Instance.UpdateWeightValues(NUranoIoHandler.Instance.GetWeight());
    }
}