using System.IO.IsolatedStorage;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using VarolaPesaCli.Models;

namespace VarolaPesaCli.Domain;

public static class PrintHandler
{

    public static ImmediateNetworkPrinter ConnectNetworkPrinter(string hostname, int port, string printerName)
    {
        var printer = new ImmediateNetworkPrinter(new ImmediateNetworkPrinterSettings()
        {
            ConnectionString = $"{hostname}:{port}",
            PrinterName = printerName
        });
        return printer;
    }
    public static async void PrintWeight(Weight weight, ImmediateNetworkPrinter immediateNetworkPrinter)
    {
        var e = new EPSON();
        await immediateNetworkPrinter.WriteAsync( // or, if using and immediate printer, use await printer.WriteAsync
        ByteSplicer.Combine(
            e.CenterAlign(),
            e.PrintLine(""),
            e.PrintLine(""),
            e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth),
            e.PrintLine("Varola"),
            e.PrintLine(""),
            e.PrintLine(""),

            e.PrintLine(""),
            e.PrintLine($"Peso: {weight.WeightValue:N2}Kg"),
            e.PrintLine($"Total: {weight.WeightValue * 46:N2}R$"),
            e.PrintLine(""),
            e.PrintLine(""),

            e.SetBarcodeHeightInDots(360),
            e.SetBarWidth(BarWidth.Default),
            e.SetBarLabelPosition(BarLabelPrintPosition.None),
            e.PrintBarcode(BarcodeType.CODE128, BarcodeNumber(weight)),
            e.PrintLine(""),
            e.PrintLine(""),

            e.FullCut()
        )
        );
    }

    // Define a callback method.
    static void StatusChanged(object sender, EventArgs ps)
    {
        var status = (PrinterStatusEventArgs)ps;
        Console.WriteLine($"Status: {status.IsPrinterOnline}");
        Console.WriteLine($"Has Paper? {status.IsPaperOut}");
        Console.WriteLine($"Paper Running Low? {status.IsPaperLow}");
        Console.WriteLine($"Cash Drawer Open? {status.IsCashDrawerOpen}");
        Console.WriteLine($"Cover Open? {status.IsCoverOpen}");
    }

    public static string BarcodeNumber(Weight weight)
    {
        string result = "";
        result += "2111111";
        decimal value = Math.Round(weight.WeightValue * 46, 2);
        string valueString = value.ToString("F2");
        string fixPlacesValueString = "";
        string noVirgulaString = valueString.Replace(",", "");
        int lenght = noVirgulaString.Length;
        if (lenght < 5)
        {
            for (int i = 0; i < 5 - lenght; i++)
            {
                fixPlacesValueString += "0";
            }
        }
        fixPlacesValueString += noVirgulaString;
        result += fixPlacesValueString;
        int odd = 0;
        int even = 0;
        bool isOdd = true;
        foreach (char c in result)
        {
            int num = Convert.ToInt32(c);
            if (isOdd)
            {
                odd += num;
            }
            else
                even += num;
            isOdd = !isOdd;
        }
        even *= 3;
        int dvNum = odd + even;
        int remainder = dvNum % 10;
        dvNum += remainder;
        string dv = Convert.ToString(remainder);

        result += dv;
        return result;
    }
}