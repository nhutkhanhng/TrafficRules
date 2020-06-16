// Decompiled with JetBrains decompiler
// Type: System.Reflection.Internal.ObjectPool`1
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 21BBF68D-61D6-4A2A-A5C3-E180E6B6706D
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Core.dll

using System.Collections.Concurrent;
using System.Threading;


/// <summary>
/// This class as same as ObjectPool In .netCore
/// </summary>
namespace System.Reflection.Internal
{
    internal sealed class ObjectPool<T> where T : class
    {   
        private readonly ObjectPool<T>.Element[] _items;
        private readonly Func<T> _factory;

        internal ObjectPool(Func<T> factory)
          : this(factory, Environment.ProcessorCount * 2)
        {
        }

        internal ObjectPool(Func<T> factory, int size)
        {
            this._factory = factory;
            this._items = new ObjectPool<T>.Element[size];
        }

        private T CreateInstance()
        {
            return this._factory();
        }

        internal T Allocate()
        {
            // Chổ này thật sự hiệu quả không ta ? - 14/6/2020.
            // Chổ này thiệt sự rất hiệu quả =]]. 15/6/2020.
            ObjectPool<T>.Element[] items = this._items;
            T instance;
            for (int index = 0; index < items.Length; ++index)
            {
                instance = items[index].Value;
                if ((object)instance != null && (object)instance == (object)Interlocked.CompareExchange<T>(ref items[index].Value, default(T), instance))
                    return instance;
            }

            instance = this.CreateInstance();
            return instance;
        }

        internal void Free(T obj)
        {
            ObjectPool<T>.Element[] items = this._items;
            for (int index = 0; index < items.Length; ++index)
            {
                if ((object)items[index].Value == null)
                {
                    items[index].Value = obj;
                    break;
                }
            }
        }

        // Why use struct in here ?????. I dont know that why
        private struct Element
        {
            internal T Value;
        }
    }
}
