﻿// ---------------------------------------------------------------------
//This file is part of DotNetWorkQueue
//Copyright © 2016 Brian Lehnen
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
namespace DotNetWorkQueue.Logging
{
    /// <summary>
    /// A colored console log provider
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ColoredConsoleLogProvider : ILogProvider
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor>
            {
                {LogLevel.Fatal, ConsoleColor.Red},
                {LogLevel.Error, ConsoleColor.Yellow},
                {LogLevel.Warn, ConsoleColor.Magenta},
                {LogLevel.Info, ConsoleColor.White},
                {LogLevel.Debug, ConsoleColor.Gray},
                {LogLevel.Trace, ConsoleColor.DarkGray}
            };

        /// <summary>
        /// Gets the specified named logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>
        /// The logger reference.
        /// </returns>
        public Logger GetLogger(string name)
        {
            return (logLevel, messageFunc, exception, formatParameters) =>
            {
                if (messageFunc == null)
                {
                    if (logLevel == LogLevel.Trace)
                        return false;

                    return true; // All log levels are enabled except for trace
                }

                ConsoleColor consoleColor;
                if (Colors.TryGetValue(logLevel, out consoleColor))
                {
                    var originalForground = Console.ForegroundColor;
                    try
                    {
                        Console.ForegroundColor = consoleColor;
                        WriteMessage(logLevel, name, messageFunc, formatParameters, exception);
                    }
                    finally
                    {
                        Console.ForegroundColor = originalForground;
                    }
                }
                else
                {
                    WriteMessage(logLevel, name, messageFunc, formatParameters, exception);
                }

                return true;
            };
        }

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="name">The name.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="formatParameters">The format parameters.</param>
        /// <param name="exception">The exception.</param>
        private static void WriteMessage(
            LogLevel logLevel,
            string name,
            Func<string> messageFunc,
            object[] formatParameters,
            Exception exception)
        {
            if (formatParameters == null || formatParameters.Length == 0)
            {
                var message = messageFunc();
                if (exception != null)
                {
                    message = message + "|" + exception;
                }
                Console.WriteLine("{0} | {1} | {2} | {3}", DateTime.UtcNow, logLevel, name, message);
            }
            else
            {
                var message = string.Format(CultureInfo.InvariantCulture, messageFunc(), formatParameters);
                if (exception != null)
                {
                    message = message + "|" + exception;
                }
                Console.WriteLine("{0} | {1} | {2} | {3}", DateTime.UtcNow, logLevel, name, message);
            }
        }

        /// <summary>
        /// Opens a nested diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="message">The message to add to the diagnostics context.</param>
        /// <returns>
        /// A disposable that when disposed removes the message from the context.
        /// </returns>
        public IDisposable OpenNestedContext(string message)
        {
            return NullDisposable.Instance;
        }

        /// <summary>
        /// Opens a mapped diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <returns>
        /// A disposable that when disposed removes the map from the context.
        /// </returns>
        public IDisposable OpenMappedContext(string key, string value)
        {
            return NullDisposable.Instance;
        }

        /// <summary>
        /// A 'null' object that won't complain if dispose is called
        /// </summary>
        private class NullDisposable : IDisposable
        {
            internal static readonly IDisposable Instance = new NullDisposable();

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            { }
        }
    }
}
