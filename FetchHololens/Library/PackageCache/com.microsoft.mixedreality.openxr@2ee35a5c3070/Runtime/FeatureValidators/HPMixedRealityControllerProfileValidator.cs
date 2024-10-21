// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;
using static UnityEngine.XR.OpenXR.Features.OpenXRFeature;

#if UNITY_OPENXR_1_8_OR_NEWER
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
using LegacyProfile = Microsoft.MixedReality.OpenXR.HPMixedRealityControllerProfile;
using NewProfile = UnityEngine.XR.OpenXR.Features.Interactions.HPReverbG2ControllerProfile;
#endif

namespace Microsoft.MixedReality.OpenXR
{
    public class HPMixedRealityControllerProfileValidator : MonoBehaviour
    {
        internal static void GetValidationChecks(OpenXRFeature feature, List<ValidationRule> results, BuildTargetGroup targetGroup)
        {
#if UNITY_OPENXR_1_8_OR_NEWER
            results.Add(new ValidationRule(feature)
            {
                message = "This profile in Mixed Reality OpenXR package is deprecated. " +
                    "Replace it with the `HP Reverb G2 Controller Profile` from Unity OpenXR package.",
                error = true,
                checkPredicate = () =>
                {
                    FeatureHelpers.RefreshFeatures(targetGroup);
                    OpenXRSettings openxrSettings = OpenXRSettings.Instance;
                    return openxrSettings != null && !FeatureValidatorHelpers.IsFeatureEnabled<LegacyProfile>(openxrSettings);
                },
                fixIt = () =>
                {
                    OpenXRSettings openxrSettings = OpenXRSettings.Instance;
                    if (openxrSettings != null)
                    {
                        FeatureValidatorHelpers.DisableFeature<LegacyProfile>(openxrSettings);
                        FeatureValidatorHelpers.EnableFeature<NewProfile>(openxrSettings);
                    }
                }
            });
#endif
        }
    }
}

#endif