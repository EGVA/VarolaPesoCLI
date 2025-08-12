using System.IO.Ports;
using System.Text.RegularExpressions;
using VarolaPesaCli.Domain;
using VarolaPesaCli.Models;
using VarolaPesaCli.Ui;
using System.Text;

namespace VarolaPesaCli;

public class UranoIoHandler
{
    private string? _receivedDataString;
    private Weight? _lastResult;
    public bool Stop = false;
    private SerialPort? _port = null;
    private readonly int _scaleReadRate = 1000;
    public readonly int SameResultCountBeforePrint = 2;
    public int SameResultCount = 0;
    public bool finishedReading = true;
    public bool render = false;
    private Queue<byte> stringBytes = new Queue<byte>();

    public string[] GetAllPorts()
    {
        return SerialPort.GetPortNames();
    }

    public void RequestScale()
    {
        if (_port != null)
        {
            while (!Stop)
            {
                byte[] handshake = new byte[1] { 0x04 };

                _port.Write(handshake, 0, 1);
            }
        }
    }

    public void OpenPort(string portName)
    {
        _port = new SerialPort(portName, 9600);
        _port.Close();
        try
        {
            if (!_port.IsOpen)
                _port.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Houve um erro ao tentar abrir a porta {_port.PortName}");
            throw;
        }
        finally
        {
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEventHandler);
        }
    }

    private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        try
        {
            if (stringBytes.Count > 150)
                stringBytes.Clear();
            byte[] data = new byte[sp.BytesToRead];
            sp.Read(data, 0, data.Length);
            if (data.Length > 32)
                return;
            data.ToList().ForEach(b => stringBytes.Enqueue(b));
            if (render)
            {
                Console.WriteLine(_receivedDataString);
                render = false;
                stringBytes.Clear();
            }

            if (data.Length == 22)
            {
                _receivedDataString = Convert(stringBytes.ToArray());
                render = true;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }


    // Now works for given string: : 00/00/00      TARA:   0.000kg       PESO L:  0.000kg      R$/kg:      0.00      TOTAL R$:      0.00200000000000
    public Weight? ParseLastResultNumbers()
    {
        Console.WriteLine(_receivedDataString);
        const string pattern = @"\d+(\.\d+)?";
        MatchCollection matches = Regex.Matches(_receivedDataString!, pattern);
        // Check if it only read the date number.
        if (matches.Count <= 3) return null;
        decimal weight = 0;
        decimal price = 0;
        decimal total = 0;
        decimal tara = 0;
        try
        {
            decimal.TryParse(matches[8].Value, out tara);
            decimal.TryParse(matches[9].Value, out weight);
            decimal.TryParse(matches[5].Value, out price);
            decimal.TryParse(matches[1].Value, out total);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{matches.Count}");
            Console.WriteLine($"Erro ao tentar converter resultado da balanca em objeto. {e} ");
            return null;
        }

        if (_lastResult != null && weight == _lastResult.WeightValue)
        {
            if (SameResultCount > SameResultCountBeforePrint)
            {
                SameResultCount = 0;
                PrintHandler.PrintWeight(_lastResult);
            }

            SameResultCount++;
        }

        Weight result = new(weight, price, total, tara);
        _lastResult = result;
        return result;
    }

    string Convert(byte[] data)
    {
        char[] characters = data.Select(b => (char)b).ToArray();
        return new string(characters);
    }
}