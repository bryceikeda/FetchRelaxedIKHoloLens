// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Microsoft.MixedReality.OpenXR
{
    /// <summary>
    /// Add the EyeLevelSceneOrigin component to the scene, it will automatically
    /// switch the Unity's scene origin to an eye level experiences.
    /// It will try to use "Unbounded" origin mode when it's supported.
    /// </summary>
    public class EyeLevelSceneOrigin : MonoBehaviour
    {
        private XRInputSubsystem m_inputSubsystem;
        private ulong m_currentSession = 0;

        private static XRInputSubsystem GetXRInputSubsystem()
        {
            XRGeneralSettings xrSettings = XRGeneralSettings.Instance;
            if (xrSettings != null)
            {
                XRManagerSettings xrManager = xrSettings.Manager;
                if (xrManager != null)
                {
                    XRLoader xrLoader = xrManager.activeLoader;
                    if (xrLoader != null)
                    {
                        return xrLoader.GetLoadedSubsystem<XRInputSubsystem>();
                    }
                }
            }
            return null;
        }

        private void Update()
        {
            if (m_currentSession != OpenXRContext.Current.Session)
            {
                m_currentSession = OpenXRContext.Current.Session;

                if (m_inputSubsystem != null)
                {
                    m_inputSubsystem.trackingOriginUpdated -= XrInputSubsystem_trackingOriginUpdated;
                    m_inputSubsystem = null;    // reset input subsystem reference on a new OpenXR session.
                }
            }

            // Lazy initialize input subsystem.
            if (m_inputSubsystem == null && OpenXRContext.Current.IsSessionRunning)
            {
                m_inputSubsystem = GetXRInputSubsystem();
                if (m_inputSubsystem != null)
                {
                    EnsureSceneOriginAtEyeLevel(m_inputSubsystem);
                    m_inputSubsystem.trackingOriginUpdated += XrInputSubsystem_trackingOriginUpdated;
                }
            }
        }

        private void XrInputSubsystem_trackingOriginUpdated(XRInputSubsystem xrInputSubsystem)
        {
            if (OpenXRContext.Current.IsSessionRunning && xrInputSubsystem == m_inputSubsystem)
            {
                EnsureSceneOriginAtEyeLevel(m_inputSubsystem);
            }
        }

        private static void EnsureSceneOriginAtEyeLevel(XRInputSubsystem xrInputSubsystem)
        {
            TrackingOriginModeFlags currentMode = xrInputSubsystem.GetTrackingOriginMode();
            TrackingOriginModeFlags desiredMode = GetDesiredTrackingOriginMode(xrInputSubsystem);
            bool isEyeLevel = currentMode == TrackingOriginModeFlags.Device || currentMode == TrackingOriginModeFlags.Unbounded;
            if (!isEyeLevel || currentMode != desiredMode)
            {
                Debug.Log($"EyeLevelSceneOrigin: TrySetTrackingOriginMode to {desiredMode}");
                if (!xrInputSubsystem.TrySetTrackingOriginMode(desiredMode))
                {
                    Debug.LogWarning($"EyeLevelSceneOrigin: Failed to set tracking origin to {desiredMode}.");
                }
            }
        }

        private static TrackingOriginModeFlags GetDesiredTrackingOriginMode(XRInputSubsystem xrInputSubsystem)
        {
            TrackingOriginModeFlags supportedFlags = xrInputSubsystem.GetSupportedTrackingOriginModes();
            TrackingOriginModeFlags targetFlag = TrackingOriginModeFlags.Device;   // All OpenXR runtime must support LOCAL space

            if (supportedFlags.HasFlag(TrackingOriginModeFlags.Unbounded))
            {
                targetFlag = TrackingOriginModeFlags.Unbounded;
            }

            return targetFlag;
        }
    }
}