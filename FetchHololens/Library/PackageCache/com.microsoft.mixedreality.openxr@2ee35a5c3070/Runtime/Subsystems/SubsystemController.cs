// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.OpenXR
{
    internal delegate void OpenXRContextEvent(IOpenXRContext sender, EventArgs args);
    internal delegate void OpenXRContextEvent<TEventArgs>(IOpenXRContext sender, TEventArgs args) where TEventArgs : EventArgs;

    internal interface IOpenXRContext
    {
        ulong Instance { get; }
        ulong SystemId { get; }
        ulong Session { get; }
        bool IsSessionRunning { get; }
        XrSessionState SessionState { get; }
        ulong SceneOriginSpace { get; }

        event OpenXRContextEvent InstanceCreated;       // after instance is created
        event OpenXRContextEvent InstanceDestroying;    // before instance is destroyed
        event OpenXRContextEvent SessionCreated;        // after session is created
        event OpenXRContextEvent SessionDestroying;     // before session is destroyed
        event OpenXRContextEvent SessionBegun;          // after session is begun
        event OpenXRContextEvent SessionEnding;         // before session is ended
    }

    internal interface ISubsystemPlugin
    {
        void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id)
            where TDescriptor : ISubsystemDescriptor
            where TSubsystem : ISubsystem;
        void StartSubsystem<T>() where T : class, ISubsystem;
        void StopSubsystem<T>() where T : class, ISubsystem;
        void DestroySubsystem<T>() where T : class, ISubsystem;
    }

    internal abstract class SubsystemController
    {
        protected readonly IOpenXRContext Context;

        public SubsystemController(IOpenXRContext context)
        {
            Context = context;
        }

        public virtual void OnSubsystemCreate(ISubsystemPlugin plugin)
        {
        }
        public virtual void OnSubsystemStart(ISubsystemPlugin plugin)
        {
        }
        public virtual void OnSubsystemStop(ISubsystemPlugin plugin)
        {
        }
        public virtual void OnSubsystemDestroy(ISubsystemPlugin plugin)
        {
        }

        internal static SubsystemController CreateFromInternalType(string fullName, NativeLibToken token, IOpenXRContext context)
        {
            Type type = FindInternalPackageType<SubsystemController>(fullName);

            if (type != null)
            {
                // Throw exceptions if the type is not configured correctly
                var ctor = type.GetConstructor(new Type[] { typeof(NativeLibToken), typeof(IOpenXRContext) });
                if (ctor == null)
                {
                    throw new Exception($"Type {type.FullName} does not have a constructor that takes a NativeLibToken and an IOpenXRContext");
                }

                return (SubsystemController)ctor.Invoke(new object[] { token, context });
            }

            return null;
        }

        private static Type FindInternalPackageType<T>(string fullName)
        {
            Type subsystemControllerType = typeof(T);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Microsoft.MixedReality.OpenXR.Internal"))
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (subsystemControllerType.IsAssignableFrom(type) &&
                            type.FullName == fullName)
                        {
                            return type;
                        }
                    }
                }
            }
            return null;
        }
    }
}
