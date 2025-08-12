using System.IO.Ports;
using VarolaPesaCli.Models;

namespace VarolaPesaCli.Ui;

public static class RenderUtils
{
    public static string PortOptions(UranoIoHandler uranoIoHandler)
    {
        string[] ports = [];

        while (ports.Length == 0)
        {
            ports = uranoIoHandler.GetAllPorts();
            if (ports.Length == 0)
            {
                Console.WriteLine("Nenhuma porta serial detectada, verifique os dispositivos e tente novamente.");
                Console.WriteLine("Buscando novamente em 5 segundos");
                Thread.Sleep(5000);
            }
        }

        Console.WriteLine("Selecione uma porta");
        for (int i = 0; i < ports.Length; i++)
        {
            Console.WriteLine($"{i} : Porta: {ports[i]}");
        }
        // Ask user to select a serial port.
        string input;
        int serialPortIndex = -1;
        bool validSerialPortInput = false;

        while (!validSerialPortInput)
        {
            input = Console.ReadLine();
            if (int.TryParse(input, out serialPortIndex))
            {
                if (serialPortIndex >= 0 && serialPortIndex < ports.Length)
                {
                    validSerialPortInput = true;
                }
                else
                {
                    Console.WriteLine($"Digite um numero entre 0 e {ports.Length} para selecionar uma porta valida.");
                }
            }
            else
            {
                Console.WriteLine($"Digite um numero entre 0 e {ports.Length} para selecionar uma porta valida.");
            }
        }

        Console.WriteLine($"Executando VarolaPesa na porta {ports[serialPortIndex]}");
        DrawSeparator();
        return ports[serialPortIndex];
        
    }

    public static void RenderWeight(UranoIoHandler uranoIoHandler)
    {
        Weight? weight = uranoIoHandler.ParseLastResultNumbers();
        if(weight != null)
        {
            
            //Console.Clear();
            Console.WriteLine("Leitura da Balanca: ");
            Console.WriteLine($"Peso: {weight.WeightValue:N2}kg");
            Console.WriteLine($"Tara: {weight.Tara:N2}kg");
            Console.WriteLine($"Preco: {weight.Price:N2}R$");
            //Console.WriteLine($"{uranoIoHandler.SameResultCount}");
            DrawSeparator();
        }
    }

    public static void DrawSeparator()
    {
        Console.WriteLine("--------------------");
    }
}