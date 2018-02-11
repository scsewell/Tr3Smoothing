using System;
using Gtk;

namespace SoSmooth
{
    public class Program
    {
        /// <summary>
        /// The name of the application.
        /// </summary>
        public const string NAME = "SoSmooth";
        
        public const string ARG_SILENT      = "-s";
        public const string ARG_HEADLESS    = "-h";
        public const string ARG_FILE        = "-file";

        /// <summary>
        /// The main method, called as the executable is started. The
        /// STAThread attibute is used to ensure thread safety for UI
        /// related components.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        private static void Main(string[] args)
        {
            // listen for any unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            
            // don't log to the console if silent specified
            Logger.SetEchoToConsole(!LineOptionParser.HasFlag(args, ARG_SILENT));
            
            // print the system information
            string os = Environment.OSVersion.VersionString;
            string platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            Logger.Info("Running on " + os + " " + platform);
            
            bool headless = LineOptionParser.HasFlag(args, ARG_HEADLESS);
            if (headless)
            {
                Logger.Info("Starting in headless mode");
            }

            string filePath;
            if (LineOptionParser.GetValueAsString(args, ARG_FILE, out filePath))
            {
                Model model = Tr3FileHandler.Instance.Read(filePath);
            }

            // launch the UI if not using headless mode
            if (!headless)
            {
                Logger.Info("Starting in UI mode");

                Application.Init();
                MainWindow window = new MainWindow();
                Application.Run();
            }

            Logger.Info("Exiting application...");
        }

        /// <summary>
        /// Called when an exception is not caught to log where it occured.
        /// </summary>
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Exception(e.ExceptionObject);
        }
    }
}
