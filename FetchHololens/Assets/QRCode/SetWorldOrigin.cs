using UnityEngine;

public class SetWorldOrigin : MonoBehaviour, ISetWorldOriginResponse
{
    public RotationScriptable origin; 
    [SerializeField] GameObject world;
    [SerializeField] SetWorldOriginEvent setWorldOriginEvent;
    int count = 0; 
    public void OnSetWorldOriginEvent(Transform transform)
    {
        world.transform.SetPositionAndRotation(transform.position, transform.rotation* (new Quaternion(.5f, .5f, .5f, .5f)));
        if (count == 20)
        {
            count = 0;
            gameObject.SetActive(false); 
        }
        count++;
    }

    private void OnEnable()
    {
        setWorldOriginEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        setWorldOriginEvent.UnregisterListener(this);
    }

}
