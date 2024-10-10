using UnityEngine;

[CreateAssetMenu(menuName = "Event/SetWorldOriginEvent"), System.Serializable]
public class SetWorldOriginEvent : EventBase<ISetWorldOriginResponse>
{
    public void Raise(Transform transform)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnSetWorldOriginEvent(transform);
    }
}
