/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Collections;
using System.Collections.Generic;

namespace Shinobytes.Core.Text
{
    public class Csv : IEnumerable<CsvRow>
    {
        private readonly List<CsvRow> rows = new List<CsvRow>();

        public CsvRow this[int row]
        {
            get { return rows[row]; }
            set { rows[row] = value; }
        }

        public int RowCount => rows.Count;

        public void Add(CsvRow row)
        {
            rows.Add(row);
        }

        public void Remove(CsvRow row)
        {
            rows.Remove(row);
        }

        public void RemoveAt(int index)
        {
            rows.RemoveAt(index);
        }

        public IEnumerator<CsvRow> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}