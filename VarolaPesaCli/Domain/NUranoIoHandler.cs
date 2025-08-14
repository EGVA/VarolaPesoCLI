using System.IO.Ports;
using System.Text.RegularExpressions;
using VarolaPesaCli.Models;
using VarolaPesaCli.Ui;
using static System.Text.RegularExpressions.Regex;

namespace VarolaPesaCli.Domain;


public static partial class MyRegex
{
    [GeneratedRegex(@"\d+(\,\d+)?")]
    public static partial Regex GetNumberRegex();
}

public class NUranoIoHandler
{
    #region Singleton
    private static readonly NUranoIoHandler instance = new NUranoIoHandler();
    static NUranoIoHandler()
    {
    }

    private NUranoIoHandler()
    {
    }

    public static NUranoIoHandler Instance
    {
        get { return instance; }
    }
    
    #endregion Singleton
    
    // Request parameter specified on Urano scale manual.
    private static readonly byte[] RequestByte = [0x04];
    
    public string SerialPortName = "";
    
    private SerialPort _serialPort;
    // Last weight output reported from scale.
    private Weight? _lastScaleResult = null;
        
    public static string[] GetAllPorts()
    {
        return SerialPort.GetPortNames();
    }
    
    public Weight? GetWeight()
    {
        return _lastScaleResult;
    }

    private void SetWeight(string readString)
    {
        _lastScaleResult = ParseWeight(readString);
        ClassesNotifier.Instance.OnScaleOutput();
    }
    
    private static Weight ParseWeight(string readString)
    {
        var myRegex = MyRegex.GetNumberRegex();
        var matches = myRegex.Match(readString);
        
        decimal weight = 0;
        decimal price = 0;
        decimal total = 0;
        decimal tara = 0;
        
        if (matches.Success)
        {
            // Use explicit variables for each parsed value
            if (decimal.TryParse(matches.Groups[0].Value, out tara) &&
                decimal.TryParse(matches.Groups[2].Value, out weight) &&
                decimal.TryParse(matches.Groups[3].Value, out price) &&
                decimal.TryParse(matches.Groups[5].Value, out total)) {
              
        }
        
        }
        var result = new Weight(weight, price, total, tara);
        return result;
    }

    public void OpenPort()
    {
        _serialPort = new SerialPort
        {
            PortName = SerialPortName,
            BaudRate = 9600,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.Two,
            ReadTimeout = 1000,
            WriteTimeout = 1000
        };

        try
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }
        catch (Exception e)
        {
            RenderSpectreUi.Instance.ShowException(e);
            throw;
        }
        finally
        {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEventHandler);
        }
    }
    public void SendIoRequest()
    {
        if (!_serialPort.IsOpen) return;
        try
        {
            _serialPort.Write(RequestByte, 0, RequestByte.Length);
        }
        catch (Exception e)
        {
            RenderSpectreUi.Instance.ShowException(e);
            throw;
        }
    }

    private static void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
    {
        var sp = (SerialPort)sender;

        try
        {
            Instance.SetWeight(sp.ReadExisting());
        }
        catch (Exception exception)
        {
            RenderSpectreUi.Instance.ShowException(exception);
            throw;
        }
    }
}