/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shinobytes.Core.Text
{
    public class CsvRow : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> data = new Dictionary<string, string>();
        //private HashSet<string> headers = new HashSet<string>();

        public string this[string columnKey]
        {
            get
            {
                if (data.ContainsKey(columnKey))
                    return data[columnKey];
                data.Add(columnKey, null);
                return null;
            }
            set
            {
                if (data.ContainsKey(columnKey))
                    data[columnKey] = value;
                else
                    data.Add(columnKey, value);
            }
        }

        public string this[int column]
        {
            get
            {
                if (column < data.Keys.Count)
                {
                    return this[data.Keys.ToArray()[column]];
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (column >= data.Keys.Count)
                    throw new IndexOutOfRangeException();

                this[data.Keys.ToArray()[column]] = value;
            }
        }

        public int ColumnCount => data.Count;

        public string[] ColumnKeys => data.Keys.ToArray();

        public string[] ColumnValues => data.Values.ToArray();

        public void Add(string key, string value)
        {
            data.Add(key, value);
        }

        public void Remove(string key)
        {
            data.Remove(key);
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}