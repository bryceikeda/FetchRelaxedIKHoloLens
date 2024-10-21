// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Microsoft.MixedReality.OpenXR
{
    internal static class FeatureValidatorHelpers
    {
        internal static bool IsFeatureEnabled<T>(OpenXRSettings openxrSettings) where T : OpenXRFeature
        {
            var feature = openxrSettings.GetFeature<T>();
            return feature != null && feature.enabled;
        }

        internal static void EnableFeature<T>(OpenXRSettings openxrSettings) where T : OpenXRFeature
        {
            var feature = openxrSettings.GetFeature<T>();
            if (feature != null)
            {
                feature.enabled = true;
            }
        }

        internal static void DisableFeature<T>(OpenXRSettings openxrSettings) where T : OpenXRFeature
        {
            var feature = openxrSettings.GetFeature<T>();
            if (feature != null)
            {
                feature.enabled = false;
            }
        }
    }
}
#endif