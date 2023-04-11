using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldersWatcher
{
    public class FoldersWatcher
    {
        private static readonly List<string> settings = ConfigurationManager.AppSettings.AllKeys.ToList()!;
        private static readonly List<string> folders = new();
        private static readonly string filePath = AppDomain.CurrentDomain.BaseDirectory; // FOLDER OF THE CURRENT PROJECT/APPLICATION
        //private static readonly string folderLogs = filePath + "\\folderlogs"; // TO GROUP THE LOGS IN A FOLDER UNDER THE CURRENT AppDomain FOLDER

        private readonly List<Thread> threads = new(); // TO KEEP TRACK OF THE THREADS WE CREATED

        private readonly Logger logger = LogManager.GetCurrentClassLogger(); // NLOGGER

        /// <summary>
        /// Start method of the service, checks logs's folder existence and iterates over the keys from appsettings, creates new threads and starts them
        /// </summary>
        public void Start()
        {
            try
            {
                int count = 0;

                //if (!Directory.Exists(folderLogs))
                //{
                //    logger.Info("Thread logs's folder doesn't exist, creating new one");
                //    Directory.CreateDirectory(folderLogs);
                //}

                logger.Info("Starting Watcher");

                foreach (string s in settings)
                {
                    folders.Add(ConfigurationManager.AppSettings[s]!);
                    logger.Info("Adding folder: " + s);
                }

                //for (int i = 0; i < settings.Count; i++)
                //{
                //    int x = i; // LAMBDA PITFALL
                //    threads.Add(new Thread(() => Watch(folders.ElementAt(x))));
                //    threads.ElementAt(x).Name = "Thread " + (x + 1);
                //}

                foreach (string s in folders)
                {
                    threads.Add(new Thread(() => Watch(s)));
                    threads.ElementAt(count).Name = "Thread " + (count + 1);
                    count++;
                    logger.Info("Creating thread " + count);
                }

                //threads.Add(new Thread(() => Watch(folders[0])));
                //threads.Add(new Thread(() => Watch(folders[1])));
                //threads[0].Name = "Thread 1";
                //threads[1].Name = "Thread 2";

                foreach (Thread t in threads)
                {
                    t.Start();
                    Console.WriteLine("Starting thread: " + t.Name);
                    logger.Info("Starting thread: " + t.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex.Message);
            }
        }

        public void Stop()
        {
            Console.WriteLine("Stopping...");
            foreach (Thread t in threads)
            {
                t.Interrupt();
                logger.Info("Stopping service");
            }
        }

        /// <summary>
        /// Method that calls the <see cref="FileSystemEventHandler"/> based on the raised events.
        /// </summary>
        /// <param name="folderPath"></param>
        private void Watch(string folderPath)
        {
            logger.Info("Watching folder: " + folderPath);

            FileSystemWatcher watcher = new()
            {
                Path = folderPath,

                NotifyFilter = NotifyFilters.LastWrite
                             | NotifyFilters.CreationTime
                             | NotifyFilters.LastAccess
                             | NotifyFilters.Attributes
                             | NotifyFilters.Size
                             | NotifyFilters.FileName
                             | NotifyFilters.DirectoryName,
                Filter = "*.*"
            };
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// OnCreated event of <see cref="FileSystemEventHandler"/>, logs the event's filepath, type, name and time to a txt file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                string[] pathing = e.FullPath.Split(Path.DirectorySeparatorChar);
                string logEvent = $"Source: {pathing[^2]} | Event Type: {e.ChangeType} | Name: {e.Name} | {DateTime.Now} | Filepath: {e.FullPath}";
                Console.WriteLine(logEvent);
                //File.AppendAllText($"{folderLogs}\\{pathing[^2]}.txt", logEvent + Environment.NewLine);
                logger.Info("New file created: " + logEvent);
            }
            catch (Exception ex)
            {
                //File.AppendAllText(folderLogs + "\\errorLog.txt", $"{ex.Source} {ex.Message}" + Environment.NewLine);
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// OnChanged event of <see cref="FileSystemEventHandler"/>, logs the event's filepath, type, name and time to a txt file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                string[] pathing = e.FullPath.Split(Path.DirectorySeparatorChar);
                string logEvent = $"Source: {pathing[^2]} | Event Type: {e.ChangeType} | Name: {e.Name} | {DateTime.Now} | Filepath: {e.FullPath}";
                Console.WriteLine(logEvent);
                //File.AppendAllText($"{folderLogs}\\{pathing[^2]}.txt", logEvent + Environment.NewLine);
                logger.Info("File changed: " + logEvent);
            }
            catch (Exception ex)
            {
                //File.AppendAllText(folderLogs + "\\errorLog.txt", $"{ex.Source} {ex.Message}" + Environment.NewLine);
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// OnDeleted event of <see cref="FileSystemEventHandler"/>, logs the event's filepath, type, name and time to a txt file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                string[] pathing = e.FullPath.Split(Path.DirectorySeparatorChar);
                string logEvent = $"Source: {pathing[^2]} | Event Type:  {e.ChangeType}  | Name: {e.Name} | {DateTime.Now} | Filepath: {e.FullPath} ";
                Console.WriteLine(logEvent);
                //File.AppendAllText($"{folderLogs}\\{pathing[^2]}.txt", logEvent + Environment.NewLine);
                logger.Info("File deleted: " + logEvent);
            }
            catch (Exception ex)
            {
                //File.AppendAllText(folderLogs + "\\errorLog.txt", $"{ex.Source} {ex.Message}" + Environment.NewLine);
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// OnRenamed event of <see cref="FileSystemEventHandler"/>, logs the event's filepath, type, name and time to a txt file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                string[] pathing = e.FullPath.Split(Path.DirectorySeparatorChar);
                string logEvent = $"Source: {pathing[^2]} | Event Type:  {e.ChangeType}  | Name: {e.Name} | {DateTime.Now} | Filepath: {e.FullPath} ";
                Console.WriteLine(logEvent);
                //File.AppendAllText($"{folderLogs}\\{pathing[^2]}.txt", logEvent + Environment.NewLine);
                logger.Info("File renamed: " + logEvent);
            }
            catch (Exception ex)
            {
                //File.AppendAllText(folderLogs + "\\errorLog.txt", $"{ex.Source} {ex.Message}" + Environment.NewLine);
                logger.Error(ex.Message);
            }
        }
    }
}