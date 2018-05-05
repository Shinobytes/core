/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core.Text
{
    public class CsvParser
    {
        public static Csv FromFile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            return FromLines(System.IO.File.ReadAllLines(fileName));
        }

        public static Csv FromText(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return FromLines(text.Split('\n'));
        }

        public static Csv FromLines(string[] lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));

            var csv = new Csv();

            for (var i = 0; i < lines.Length; i++)
            {
                var row = ParseRow(lines[i]);
                if (row != null)
                {
                    csv.Add(row);
                }
            }

            return csv;
        }

        private static CsvRow ParseRow(string data)
        {
            var outrow = new CsvRow();
            var currentText = "";
            var headerIndex = 0;
            var row = data;
            if (!row.EndsWith(",")) row += ','; // just to simplify the parsing
            for (int index = 0; index < row.Length; index++)
            {
                var token = row[index];
                switch (token)
                {
                    /* not implemented */
                    case '\'':
                    case '"': continue;
                    case ',':
                    case ';':
                    case '\t':
                        outrow.Add(headerIndex.ToString(), currentText);
                        headerIndex++;
                        currentText = "";
                        continue;
                    default:
                        currentText += token;
                        continue;
                }
            }
            return outrow;
        }
    }
}