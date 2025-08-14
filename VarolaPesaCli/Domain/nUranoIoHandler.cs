using System.IO.Ports;
using VarolaPesaCli.Models;

namespace VarolaPesaCli.Domain;

public class nUranoIoHandler
{
    #region Singleton
    private static readonly nUranoIoHandler instance = new nUranoIoHandler();
    static nUranoIoHandler()
    {
    }

    private nUranoIoHandler()
    {
    }

    public static nUranoIoHandler Instance
    {
        get { return instance; }
    }
    
    #endregion Singleton

    public string serialPortName = "";
        
    public static string[] GetAllPorts()
    {
        return SerialPort.GetPortNames();
    }
    
    public Weight GetWeight()
    {
        
        return new Weight(0,0,0,0);
    }
}