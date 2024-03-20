using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cute_Codes
{
    internal class Logger
    {
        private static Logger instance;

        private static readonly object threadLock = new();

        private static DateTime startTime;

        private static string logPath = "NULL";

        public static Logger Instance
        {
            get
            {
                lock (threadLock)
                {
                    instance ??= new Logger();
                    return instance;
                }
            }
        }

        private bool initialized = false;

        Logger()
        {
            Log(ConsoleColor.Blue, "Initializing Logger...");
            InitLogger();
        }

        /// <summary>
        /// Initializes The logger instance to be used in other classes
        /// </summary>
        private void InitLogger()
        {
            try
            {
                if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");

                startTime = DateTime.Now;
                logPath = "Logs/" + startTime.ToString("MM-dd-yyyy HH-mm-ss") + ".log";
                FileStream logFileStrm = File.Create(logPath);
                logFileStrm.Close();
                initialized = true;
                Log(ConsoleColor.Green, "Successfully initialized Logger!");
            }
            catch (Exception ex)
            {
                Log(ConsoleColor.Red, $"Logger failed to properly initialize with the exception: {ex}");
                Log(ConsoleColor.Blue, "Reinitializing Logger...");
                InitLogger();
            }
        }

        /// <summary>
        /// Logs text to console and the current log file
        /// </summary>
        /// <param name="txt">Text to be displayed in the console and written to the log file</param>
        public void Log(string txt)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string time = DateTime.Now.ToString("HH:mm:ss ff");
            Console.Write(time);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");

            Console.Write(txt + "\n");


            if (!initialized) return;

            LogToFile($"[{time}] {txt}");
        }

        /// <summary>
        /// Logs colored text to console and the current log file
        /// </summary>
        /// <param name="txt">Text to be displayed in the console and written to the current log file</param>
        public void Log(ConsoleColor color, string txt)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string time = DateTime.Now.ToString("HH:mm:ss ff");
            Console.Write(time);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");

            Console.ForegroundColor = color;

            Console.Write(txt + "\n");

            Console.ForegroundColor = ConsoleColor.White;


            if (!initialized) return;

            LogToFile($"[{time}] {txt}");
        }

        /// <summary>
        /// Logs text to log file
        /// </summary>
        /// <param name="txt">Text to be written to the current log file</param>
        public void LogToFile(string txt)
        {
            if(File.Exists(logPath))
            {
                StreamWriter writer = File.AppendText(logPath);
                writer.WriteLine(txt);
                writer.Close();
            }
            else
            {
                initialized = false;
                Log(ConsoleColor.Red, "Logger failed to properly initialize");
                Log(ConsoleColor.Green, "Reinitializing Logger...");
                InitLogger();
            }
        }
    }
}
