using System.Globalization;
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
    private Weight? _lastPrintedResult = null;
    public int inSequenceSameWeightQtt = 0;
    public const int sequenceToPrint = 1;

    // Confirm if the actual measure is a repeated measure.
    public bool alreadyPrinted = false;

    public static string[] GetAllPorts()
    {
        return SerialPort.GetPortNames();
    }

    public Weight GetWeight()
    {
        return _lastScaleResult ?? new Weight(0, 0, 0, 0);
    }

    public void ClosePort()
    {
        _serialPort.Close();
    }

    private void SetWeight(string readString)
    {
        //Console.WriteLine(readString);
        Weight newWeight = ParseWeight(readString);
        if (_lastScaleResult != null && newWeight != null && newWeight.WeightValue == _lastScaleResult!.WeightValue)
        {
            inSequenceSameWeightQtt++;
        }
        else{
            inSequenceSameWeightQtt = 0;
        }
        _lastScaleResult = newWeight;
        //Console.WriteLine(newWeight.WeightValue);

        ClassesNotifier.Instance.OnScaleOutput();
    }

    // private static Weight ParseWeight(string readString)
    // {
    //     const string pattern = @"\d+(\,\d+)?";
    //     MatchCollection matches = Regex.Matches(readString!, pattern);
    //     if (matches.Count <= 3) return null;

    //     decimal weight = 0;
    //     decimal price = 0;
    //     decimal total = 0;
    //     decimal tara = 0;

    //     // for (int i = 0; i < matches.Count; i++)
    //     // {
    //     //     Console.WriteLine(matches[i].Value + " - " + i);
    //     // }
    //     // Console.ReadKey();

    //     // Use explicit variables for each parsed valu
    //     //decimal.TryParse(matches[0].Value, out tara);
    //     decimal.TryParse(matches[2].Value, out weight);
    //     decimal.TryParse(matches[3].Value, out price) ;
    //     decimal.TryParse(matches[5].Value, out total);

    //     var result = new Weight(weight, price, total, tara);
    //     return result;
    // }

    private static Weight ParseWeight(string readString)
    {
        // Console.WriteLine(readString);
        const string pattern = @"[\d.,]+";
        MatchCollection matches = Regex.Matches(readString!, pattern);
        // Check if it only read the date number.
        if (matches.Count <= 3) return null;
        decimal weight = 0;
        decimal price = 0;
        decimal total = 0;
        decimal tara = 0;
        try
        {
            decimal.TryParse(matches[7].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out tara);
            decimal.TryParse(matches[8].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out weight);
            decimal.TryParse(matches[9].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out price);
            decimal.TryParse(matches[10].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out total);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{matches.Count}");
            Console.WriteLine($"Erro ao tentar converter resultado da balanca em objeto. {e} ");
            return null;
        }

        // for (int i = 0; i < matches.Count; i++)
        // {
        //     Console.WriteLine($"{i} - {matches[i].Value}");
        // }

        Weight result = new(weight, price, total, tara);
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
        }
    }

    private static void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
    {
        var sp = (SerialPort)sender;
        Thread.Sleep(200);
        try
        {
            Instance.SetWeight(sp.ReadExisting());
        }
        catch (Exception exception)
        {
            RenderSpectreUi.Instance.ShowException(exception);
        }
    }

    public bool CanPrint()
    {
        if (_lastPrintedResult == null)
        {
            _lastPrintedResult = _lastScaleResult;
            return true;
        }
        if (_lastScaleResult != null)
        {
            if (_lastScaleResult.WeightValue == 0)
            {
                return false;
            }
            if (_lastScaleResult.WeightValue == _lastPrintedResult.WeightValue)
            {
                return false;
            }
        }
        if (inSequenceSameWeightQtt > sequenceToPrint)
        {
            _lastPrintedResult = _lastScaleResult;
            return true;
        }
        else return false;
    }
}