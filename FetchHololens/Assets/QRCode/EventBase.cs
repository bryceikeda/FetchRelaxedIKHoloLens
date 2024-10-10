using System.Collections.Generic;
using UnityEngine;

public abstract class EventBase<T> : ScriptableObject
{
    public List<T> listeners = new List<T>();
    public virtual void RegisterListener(T listener) => listeners.Add(listener);

    public virtual void UnregisterListener(T listener) => listeners.Remove(listener);
}
