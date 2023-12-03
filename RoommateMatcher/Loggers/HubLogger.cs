using System.Collections.Concurrent;

namespace RoommateMatcher.Loggers
{
    public class HubLogger
    {
        private readonly string _logFilePath;
        private readonly ConcurrentQueue<string> _logQueue;
        private readonly object _lockObj = new object();
        private bool _running;

        public HubLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
            _logQueue = new ConcurrentQueue<string>();
            _running = true;
            Task.Run(() => ProcessQueue());
        }

        public void Log(string message)
        {
            _logQueue.Enqueue($"{DateTime.UtcNow}: {message}");
        }

        private void ProcessQueue()
        {
            while (_running)
            {
                if (_logQueue.TryDequeue(out string logEntry))
                {
                    WriteToFile(logEntry);
                }
                else
                {
                    Task.Delay(100).Wait();
                }
            }
        }

        private void WriteToFile(string logEntry)
        {
            lock (_lockObj)
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
        }

        public void StopLogging()
        {
            _running = false;
        }
    }
}

