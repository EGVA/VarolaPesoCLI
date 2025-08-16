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
            e.PrintBarcode(BarcodeType.CODE128, GenerateBarcode(weight)),
            e.PrintLine(""),
            e.PrintLine(""),

            e.FullCut()
        )
        );
    }

    // c = product code p = product units  d = verification number 
    // 2 + cccccc + ppppp + d
    public static string GenerateBarcode(Weight weight)
    {
        string result = "";
        // PDV code for the product. First 2 indicates that EAN13 code represents a variable unit product
        const string productCode = "2" + "111111";

        // Round price value 2 floors
        decimal roundedProductPrice = Math.Round(weight.WeightValue * 46, 2);
        // Convert to string and force to keep 2 decimal floors
        string productPriceString = roundedProductPrice.ToString("F2");
        // Removes comma to keep EA13 pattern
        productPriceString = productPriceString.Replace(",", "");

        result += productCode;
        int lenght = productPriceString.Length;
        if (lenght < 5)
        {
            for (int i = 0; i < 5 - lenght; i++)
            {
                result += 0;
            }
        }
        result += productPriceString;


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