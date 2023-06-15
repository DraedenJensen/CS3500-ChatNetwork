/// <summary>
/// Author:    Draeden Jensen
/// Date:      04-02-2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Draeden Jensen - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Draeden Jensen, certify that this code was written from scratch and
/// we did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in the README file.
///
/// File Contents:
/// Contains a definition for a custom file logger.
/// </summary>

using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FileLogger
{
    internal class CustomFileLogger : ILogger
    {
        private string fileName;

        /// <summary>
        /// Constructor which builds a new CustomFileLogger and defines the file name and location
        /// to write to.
        /// </summary>
        public CustomFileLogger(string categoryName)
        {
            string dateTime = DateTime.Now.ToString().Replace(' ', '-').Replace(':', '-').Replace('/', '-');
            fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                           + Path.DirectorySeparatorChar
                           + $"CS3500-{categoryName}{dateTime}.log";
            Debug.WriteLine(fileName);
        }

        /// <summary>
        /// Formats the message and creates a scope. Not implemented in this class.
        /// </summary>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented in this class.
        /// </summary>
        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <para>
        /// Logs a message to the file, documenting the following: current date and time, the 
        /// current thread's ID, this message's log level, and the message describing an event.
        /// </summary>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            File.AppendAllText(fileName, $"{DateTime.Now} ({Environment.CurrentManagedThreadId}) - {logLevel.ToString().Substring(0, 5)} - {formatter(state, exception)}{Environment.NewLine}");
        }
    }
}
