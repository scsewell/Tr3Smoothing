using System;
using Gtk;

namespace SoSmooth
{
    public class Program
    {
        public const string NAME = "SoSmooth";

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

            CommandLineParser options = new CommandLineParser(args);

            // only log to the console if using verbose mode
            Logger.SetEchoToConsole(options.HasFlag("-v"));

            string os = Environment.OSVersion.VersionString;
            string platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            Logger.Info("Running on " + os + " " + platform);
            
            // launch the UI if not using headless mode
            if (!options.HasFlag("-headless"))
            {
                Logger.Info("Starting in UI mode");

                Application.Init();
                MainWindow window = new MainWindow();
                Application.Run();
            }
            else
            {
                Logger.Info("Starting in headless mode");
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
