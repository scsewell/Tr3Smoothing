using Gtk;

namespace SoSmooth
{
    public class Program
    {
        public const string NAME = "SoSmooth";

        /// <summary>
        /// The main method, called as the executable is started.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        private static void Main(string[] args)
        {
            CommandLineParser options = new CommandLineParser(args);

            // only log to the console if using verbose mode
            Logger.SetEchoToConsole(options.HasFlag("-v"));
            
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

            Logger.Info("Exiting Application...");
        }
    }
}
