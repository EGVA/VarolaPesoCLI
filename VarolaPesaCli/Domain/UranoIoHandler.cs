// using System.IO.Ports;
// using System.Text.RegularExpressions;
// using VarolaPesaCli.Domain;
// using VarolaPesaCli.Models;
// using VarolaPesaCli.Ui;
// using ESCPOS_NET;
//
// namespace VarolaPesaCli;
//
// public class UranoIoHandler
// {
//     private string? _receivedDataString;
//     private Weight? _lastResult;
//     public bool Stop = false;
//     private SerialPort? _port = null;
//     private readonly int _scaleReadRate = 1000;
//     public readonly int SameResultCountBeforePrint = 4;
//     public int SameResultCount = 0;
//     public bool finishedReading = true;
//     public bool render = false;
//     private Queue<byte> stringBytes = new Queue<byte>();
//     private decimal lastPrintedWeight = 0;
//     private bool receivedData = false;
//
//     public static string[] GetAllPorts()
//     {
//         return SerialPort.GetPortNames();
//     }
//
//     public void RequestScale()
//     {
//         if (_port != null && _port.IsOpen)
//         {
//             while (!Stop)
//             {
//                 if (Console.KeyAvailable)
//                 {
//                     ConsoleKeyInfo keyInfo = Console.ReadKey();
//                     if (keyInfo.Key == ConsoleKey.Escape)
//                         Stop = true;
//
//                 }
//                 byte[] handshake = new byte[1] { 0x04 };
//
//                 _port.Write(handshake, 0, 1);
//
//             }
//             _port.Close();
//         }
//     }
//
//
//     public void OpenPort(string portName)
//     {
//         _port = new SerialPort
//         {
//             PortName = portName,
//             BaudRate = 9600,
//             Parity = Parity.None,
//             DataBits = 8,
//             StopBits = StopBits.Two,
//             ReadTimeout = 1000,
//             WriteTimeout = 1000
//         };
//         try
//         {
//             if (!_port.IsOpen)
//                 _port.Open();
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine($"Houve um erro ao tentar abrir a porta {_port.PortName}");
//             throw;
//         }
//         finally
//         {
//             _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEventHandler);
//         }
//     }
//
//     private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
//     {
//         SerialPort sp = (SerialPort)sender;
//         try
//         {
//             receivedData = true;
//             Thread.Sleep(100);
//             _receivedDataString = sp.ReadExisting();
//             if (_lastResult != null && _lastResult.WeightValue != lastPrintedWeight)
//             {
//                 lastPrintedWeight = -1;
//             }
//             RenderUtils.RenderWeight(this);
//         }
//         catch (Exception exception)
//         {
//             Console.WriteLine(exception);
//             throw;
//         }
//     }
//
//
//     // Now works for given string: : 00/00/00      TARA:   0.000kg       PESO L:  0.000kg      R$/kg:      0.00      TOTAL R$:      0.00200000000000
//     // 20           0,000 kg      0,00 1     0,00 
//     public Weight? ParseLastResultNumbers()
//     {
//         //Console.WriteLine(_receivedDataString);
//         const string pattern = @"\d+(\,\d+)?";
//         MatchCollection matches = Regex.Matches(_receivedDataString!, pattern);
//         // Check if it only read the date number.
//         if (matches.Count <= 3) return null;
//         decimal weight = 0;
//         decimal price = 0;
//         decimal total = 0;
//         decimal tara = 0;
//
//         try
//         {
//             decimal.TryParse(matches[0].Value, out tara);
//             decimal.TryParse(matches[2].Value, out weight);
//             decimal.TryParse(matches[3].Value, out price);
//             decimal.TryParse(matches[5].Value, out total);
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine($"{matches.Count}");
//             Console.WriteLine($"Erro ao tentar converter resultado da balanca em objeto. {e} ");
//             return null;
//         }
//         if (weight == 0)
//         {
//             SameResultCount = 0;
//         }
//         if (_lastResult != null && weight == _lastResult.WeightValue && _lastResult.WeightValue != lastPrintedWeight && _lastResult.WeightValue > 0)
//         {
//             if (SameResultCount > SameResultCountBeforePrint)
//             {
//                 SameResultCount = 0;
//                 ImmediateNetworkPrinter printer = PrintHandler.ConnectNetworkPrinter("192.168.3.154", 9100, "Caixa");
//                 lastPrintedWeight = _lastResult.WeightValue;
//                 PrintHandler.PrintWeight(_lastResult, printer);
//             }
//
//             SameResultCount++;
//         }
//
//         Weight result = new(weight, price, total, tara);
//         _lastResult = result;
//         return result;
//     }
//
//     string Convert(byte[] data)
//     {
//         char[] characters = data.Select(b => (char)b).ToArray();
//         return new string(characters);
//     }
// }