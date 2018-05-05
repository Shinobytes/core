/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core
{
    public class ConsoleLogger : ILogger
    {
        private readonly object writerLock = new object();
        
        public void WriteMessage(string message)
        {
            lock (writerLock)
                Console.WriteLine(message);
        }

        public void WriteWarning(string message)
        {
            lock (writerLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public void WriteError(string message)
        {
            lock (writerLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public void WriteDebug(string message)
        {
            lock (writerLock)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public void SetTopic(string topic)
        {
            lock (writerLock)
            {
                Console.Title = topic;
            }
        }
    }
}