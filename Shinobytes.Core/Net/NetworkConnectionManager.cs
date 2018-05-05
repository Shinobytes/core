/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shinobytes.Core.Net
{
    public class NetworkConnectionManager : INetworkConnectionManager
    {
        private readonly List<INetworkConnection> connections;

        public NetworkConnectionManager()
        {
            this.connections = new List<INetworkConnection>();
        }

        public IEnumerator<INetworkConnection> GetEnumerator()
        {
            return connections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(INetworkConnection item)
        {
            connections.Add(item);
        }

        public void Clear()
        {
            connections.Clear();
        }

        public bool Contains(INetworkConnection item)
        {
            return connections.Contains(item) || connections.FirstOrDefault(i => i.InstanceIdentifier == item.InstanceIdentifier) != null;
        }

        public void CopyTo(INetworkConnection[] array, int arrayIndex)
        {
            connections.CopyTo(array, arrayIndex);
        }

        public bool Remove(INetworkConnection item)
        {
            if (!connections.Contains(item)) return false;
            if (connections.Remove(item)) return true;
            var c = connections.FirstOrDefault(i => i.InstanceIdentifier == item.InstanceIdentifier);
            return c != null && connections.Remove(c);
        }

        public int Count => connections.Count;

        public bool IsReadOnly => false;

        public int IndexOf(INetworkConnection item)
        {
            var index = connections.IndexOf(item);
            if (index >= 0) return index;

            var c = connections.FirstOrDefault(i => i.InstanceIdentifier == item.InstanceIdentifier);
            if (c == null) return -1;
            return connections.IndexOf(c);
        }

        public void Insert(int index, INetworkConnection item)
        {
            connections.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            connections.RemoveAt(index);
        }

        public INetworkConnection this[int index]
        {
            get { return connections[index]; }
            set { connections[index] = value; }
        }
    }
}