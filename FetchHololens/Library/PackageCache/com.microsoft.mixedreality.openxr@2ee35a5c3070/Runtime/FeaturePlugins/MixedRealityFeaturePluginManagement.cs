// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Microsoft.MixedReality.OpenXR
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresNativePluginDLLsAttribute : Attribute { }

    // A utility for managing connections between Mixed Reality OpenXR feature plugins and the native plugin DLL.
    internal static class MixedRealityFeaturePluginManagement
    {
        // If this is true, the NativeLib is guaranteed to be available.
        // If this is false, the NativeLib may or may not be available.
        internal static bool NativeLibAvailable { get => m_nativeLibAvailable; }
        private static bool m_nativeLibAvailable = false;

        internal static void OnFeaturePluginInitializing<TPlugin>(OpenXRFeaturePlugin<TPlugin> feature) where TPlugin : OpenXRFeaturePlugin<TPlugin>
        {
            if (!m_nativeLibAvailable)
            {
                TryInitializeNativeLibAvailable<TPlugin>(feature);
            }
        }

        private static void TryInitializeNativeLibAvailable<TPlugin>(OpenXRFeaturePlugin<TPlugin> feature) where TPlugin : OpenXRFeaturePlugin<TPlugin>
        {
            // If the OpenXR loader is not in the list of active loaders, the native DLL will not be available.
            if (XRGeneralSettings.Instance?.Manager == null || !XRGeneralSettings.Instance.Manager.activeLoaders.Any(loader => loader is OpenXRLoaderBase))
            {
                return;
            }

            // For FeaturePlugins which have the RequiresNativePluginDLLs attribute, if they are enabled, the native DLL must be available.
            if (feature.enabled && Attribute.IsDefined(typeof(TPlugin), typeof(RequiresNativePluginDLLsAttribute)))
            {
                m_nativeLibAvailable = true;
            }
        }
    }
}