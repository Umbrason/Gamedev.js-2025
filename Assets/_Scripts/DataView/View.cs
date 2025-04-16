using System;
using UnityEngine;

namespace DataView
{
    public abstract class View<T> : MonoBehaviour
    {
        protected T data;
        public T Data { 
            get => data;
            set{
                Refresh(data, value);
                data = value;
            } 
        }

        protected abstract void Refresh(T oldData, T newData);
    }
}
