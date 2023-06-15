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
/// Contains a definition for a custom file logger provider.
/// </summary>

using Microsoft.Extensions.Logging;

namespace FileLogger
{
    public class CustomFileLogProvider : ILoggerProvider
    {
        /// <inheritdoc\>
        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName);
        }

        /// <inheritdoc\>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}