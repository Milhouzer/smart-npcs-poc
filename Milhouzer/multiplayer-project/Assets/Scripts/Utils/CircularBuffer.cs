using System;
using UnityEngine;

namespace Milhouzer {
    [Serializable]
    public class CircularBuffer<T> {
        [SerializeField]
        T[] buffer;
        int bufferSize;
        
        public CircularBuffer(int bufferSize) {
            this.bufferSize = bufferSize;
            buffer = new T[bufferSize];
        }

        public CircularBuffer(CircularBuffer<T> other)
        {
            this.bufferSize = other.Count();
            buffer = new T[bufferSize];
            for (int i = 0; i < other.Count(); i++)
            {
               buffer[i] = other.Get(i); 
            }
        }
        
        public void Add(T item, int index) => buffer[index % bufferSize] = item;
        public T Get(int index) {
            return bufferSize == 0 ? buffer[index] : buffer[index % bufferSize];
        }
        public void Clear() => buffer = new T[bufferSize];
        public int Count() => bufferSize;
    }
}