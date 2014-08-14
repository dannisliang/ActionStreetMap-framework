using System;

namespace Mercraft.Infrastructure
{
    public delegate void DataEventHandler<T>(object sender, DataEventArgs<T> e);

    public class DataEventArgs<T> : EventArgs
    {
        public DataEventArgs(T data)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }
}
