using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    private void Update()
    {
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (handJointService != null)
        {
            Transform jointTransform = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Handedness.Right);

            gameObject.transform.position = jointTransform.position;
            gameObject.transform.rotation = jointTransform.rotation;
        }
    }
}
