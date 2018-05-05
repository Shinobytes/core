/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Collections.Generic;

namespace Shinobytes.Core
{
    public class SyntaxHighlightedConsoleLogger : ILogger
    {
        private const ConsoleColor NormalColor = ConsoleColor.White;
        private const ConsoleColor NumberColor = ConsoleColor.Cyan;
        private const ConsoleColor StringColor = ConsoleColor.Magenta;
        private const ConsoleColor QuouteColor = ConsoleColor.Magenta;
        private const ConsoleColor BracketColor = ConsoleColor.DarkGray;
        private const ConsoleColor ParanthesisColor = ConsoleColor.DarkGray;
        private const ConsoleColor InsideParanthesisColor = ConsoleColor.Yellow;

        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private const ConsoleColor WarningColor = ConsoleColor.Yellow;
        private const ConsoleColor DebugColor = ConsoleColor.Cyan;

        private object writerLock = new object();

        public void WriteMessage(string message)
        {
            WriteHightlightedText(GetHightlightedTextParts("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message + Environment.NewLine, LogMessageType.Normal));
        }

        public void WriteWarning(string message)
        {
            WriteHightlightedText(GetHightlightedTextParts("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] [WARNING] " + message + Environment.NewLine, LogMessageType.Warning));
        }

        public void WriteError(string message)
        {
            WriteHightlightedText(GetHightlightedTextParts("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] [ERROR] " + message + Environment.NewLine, LogMessageType.Error));
        }

        public void WriteDebug(string message)
        {
            WriteHightlightedText(GetHightlightedTextParts("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] [DEBUG] " + message + Environment.NewLine, LogMessageType.Debug));
        }

        private void WriteHightlightedText(HightligtedTextPart[] textParts)
        {
            lock (writerLock)
            {
                foreach (var part in textParts)
                {
                    if (part == null) continue;
                    if (part.IsLineBreak)
                    {
                        Console.WriteLine();
                        continue;
                    }
                    Console.ForegroundColor = part.Color;
                    if (!string.IsNullOrEmpty(part.Text)) Console.Write(part.Text);
                    Console.ResetColor();
                }
            }
        }

        public void SetTopic(string topic)
        {
            lock (writerLock)
            {
                Console.Title = topic;
            }
        }

        private HightligtedTextPart[] GetHightlightedTextParts(string message, LogMessageType type)
        {
            HightligtedTextPart currentPart = null;
            var parts = new List<HightligtedTextPart>();
            var lastColor = ConsoleColor.White;
            var isInsideString = false;
            var isInsideQuoute = false;
            var isInsideParanthesis = false;
            for (var i = 0; i < message.Length; i++)
            {
                var token = GetToken(message, i, isInsideString, isInsideParanthesis, isInsideQuoute);
                if (token == null) continue;
                var color = token.Color;
                if (token.IsQuouteStart) isInsideQuoute = true;
                if (token.IsQuouteEnd) isInsideQuoute = false;
                if (token.IsStringStart) isInsideString = true;
                if (token.IsStringEnd) isInsideString = false;
                if (token.IsParanthesisStart) isInsideParanthesis = true;
                if (token.IsParanthesisEnd) isInsideParanthesis = false;
                if (token.IsLineBreak)
                {
                    if (currentPart != null)
                    {
                        parts.Add(currentPart);
                        parts.Add(new HightligtedTextPart { IsLineBreak = true });
                        currentPart = null;
                        lastColor = color;
                        continue;
                    }
                }

                if (currentPart == null)
                {
                    currentPart = new HightligtedTextPart();
                    currentPart.Text += token.Text;
                    currentPart.Color = color;
                    lastColor = color;
                }
                else if (lastColor == color)
                {
                    currentPart.Text += token.Text;
                    if (i >= message.Length - 1)
                    {
                        parts.Add(currentPart);
                        break;
                    }
                }
                if (lastColor != color)
                {
                    parts.Add(currentPart);

                    currentPart = new HightligtedTextPart();
                    currentPart.Text += token.Text;
                    currentPart.Color = color;

                    lastColor = color;
                }
            }
            switch (type)
            {
                case LogMessageType.Error:
                    for (var i = 3; i < parts.Count; i++) parts[i].Color = ErrorColor;
                    break;
                case LogMessageType.Warning:
                    for (var i = 3; i < parts.Count; i++) parts[i].Color = WarningColor;
                    break;
                case LogMessageType.Debug:
                    for (var i = 3; i < parts.Count; i++) parts[i].Color = DebugColor;
                    break;
            }
            return parts.ToArray();
        }

        private HightligtedTextToken GetToken(string message, int index, bool insideString = false, bool insideParanthesis = false, bool insideQuoute = false)
        {
            var c = message[index];
            var i = index;
            switch (c)
            {
                case ')':
                    return new HightligtedTextToken { Color = ParanthesisColor, Text = c.ToString(), IsParanthesisEnd = true };
                case '(':
                    return new HightligtedTextToken { Color = ParanthesisColor, Text = c.ToString(), IsParanthesisStart = true };
                case '[':
                case '{':
                    return new HightligtedTextToken { Color = BracketColor, Text = c.ToString(), IsParanthesisStart = true };
                case '}':
                case ']':
                    return new HightligtedTextToken { Color = BracketColor, Text = c.ToString(), IsParanthesisEnd = true };
                case '\t':
                    return new HightligtedTextToken { IsTab = true, Color = NormalColor };
                case '\n':
                    return new HightligtedTextToken { IsLineBreak = true };
                case '\r':
                    if (index - 1 >= 0 && !GetToken(message, index - 1).IsLineBreak)
                        return new HightligtedTextToken { IsLineBreak = true };
                    break;
                case '\'':
                    if (insideParanthesis) return new HightligtedTextToken { IsQuouteEnd = true, Text = "'", Color = InsideParanthesisColor };
                    if (insideQuoute) return new HightligtedTextToken { IsQuouteEnd = true, Text = "'", Color = QuouteColor };
                    return new HightligtedTextToken { IsQuouteStart = true, Text = "'", Color = QuouteColor };
                case '"':
                    if (insideParanthesis) return new HightligtedTextToken { IsStringEnd = true, Text = "\"", Color = InsideParanthesisColor };
                    if (insideString) return new HightligtedTextToken { IsStringEnd = true, Text = "\"", Color = StringColor };
                    return new HightligtedTextToken { IsStringStart = true, Text = "\"", Color = StringColor };
                case '.':
                    if (insideParanthesis)
                        return new HightligtedTextToken { Text = c.ToString(), Color = InsideParanthesisColor };
                    if (index - 1 >= 0 && GetToken(message, index - 1).IsDigit)
                        return new HightligtedTextToken { IsDigit = true, Color = NumberColor, Text = "." };
                    return new HightligtedTextToken
                    {
                        Text = message[i].ToString(),
                        Color = insideQuoute ? QuouteColor : insideString ? StringColor : NormalColor
                    };
                case ':':
                    if (insideParanthesis) return new HightligtedTextToken { Text = c.ToString(), Color = InsideParanthesisColor };
                    return new HightligtedTextToken
                    {
                        Text = message[i].ToString(),
                        Color = insideQuoute ? QuouteColor : insideString ? StringColor : NormalColor
                    };
                default:
                    if (insideParanthesis) return new HightligtedTextToken { Text = c.ToString(), Color = InsideParanthesisColor };
                    if (char.IsDigit(c))
                    {
                        return new HightligtedTextToken
                        {
                            Text = c.ToString(),
                            IsDigit = true,
                            Color = NumberColor
                        };
                    }
                    return new HightligtedTextToken
                    {
                        Text = message[i].ToString(),
                        Color = insideQuoute ? QuouteColor : insideString ? StringColor : NormalColor
                    };
            }
            return null;
        }

        /// <summary>
        /// Used for ONE character, unless \r\n, \n, \r, \t
        /// </summary>
        private class HightligtedTextToken
        {
            public bool IsLineBreak { get; set; }
            public bool IsTab { get; set; }
            public bool IsDigit { get; set; }
            public bool IsStringStart { get; set; }
            public bool IsStringEnd { get; set; }
            public bool IsQuouteStart { get; set; }
            public bool IsQuouteEnd { get; set; }
            public bool IsParanthesisEnd { get; set; }
            public bool IsParanthesisStart { get; set; }
            public string Text { get; set; }
            public ConsoleColor Color { get; set; }

        }

        /// <summary>
        /// Used for ONE or MORE characters that share the same color.
        /// </summary>
        private class HightligtedTextPart
        {
            public bool IsLineBreak { get; set; }
            public string Text { get; set; }
            public ConsoleColor Color { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }
    }

    public enum LogMessageType
    {
        Normal,
        Warning,
        Error,
        Debug
    }
}
