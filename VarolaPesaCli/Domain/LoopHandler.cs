using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VarolaPesaCli.Ui;

namespace VarolaPesaCli.Domain
{
    public class LoopHandler
    {
        #region Singleton
        private static readonly LoopHandler instance = new LoopHandler();
        static LoopHandler()
        {
        }

        private LoopHandler()
        {
        }

        public static LoopHandler Instance
        {
            get { return instance; }
        }

        #endregion Singleton

        const int timeToReadScale = 1000;
        bool stopLoop = false;
        readonly RenderSpectreUi _uiInstance = RenderSpectreUi.Instance;
        readonly  NUranoIoHandler _uranoInstance = NUranoIoHandler.Instance;
        readonly ClassesNotifier _notifierInstance = ClassesNotifier.Instance;


        public void Run()
        {
            while (true)
            {
                // Setups the terminal layout.
                _uiInstance.Setup();
                // Show io ports options, and set portname to nUranoIoHandler
                _uiInstance.ShowWelcomeScreen();
                // Try to open port connection
                _uranoInstance.OpenPort();

                while (!stopLoop)
                {
                    if (Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Escape)
                            stopLoop = true;

                        _uranoInstance.ClosePort();
                        break;
                    }

                    // Request output from scale
                    try
                    {
                        _uranoInstance.SendIoRequest();
                    }
                    catch (Exception e)
                    {
                        _uranoInstance.ClosePort();
                        stopLoop = true;
                        _uiInstance.ShowException(e);
                    }
                    Thread.Sleep(timeToReadScale);
                }
            }
        }
    }
}