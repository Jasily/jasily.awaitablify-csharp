using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jasily.Awaitablify.Internal
{
    internal class AwaiterInfo
    {
        private static readonly TypeInfo NotifyCompletionTypeInfo = typeof(INotifyCompletion).GetTypeInfo();
        private static readonly TypeInfo CriticalNotifyCompletionTypeInfo = typeof(ICriticalNotifyCompletion).GetTypeInfo();

        private AwaiterInfo([NotNull] TypeInfo typeInfo)
        {
            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        }

        public TypeInfo TypeInfo { get; }

        /// <summary>
        /// A function that has zero parameters and maybe has return value.
        /// </summary>
        [NotNull]
        public MethodInfo GetResultMethod { get; private set; }

        /// <summary>
        /// A function that has zero parameters and has bool return value.
        /// </summary>
        [NotNull]
        public MethodInfo IsCompletedMethod { get; private set; }

        [NotNull]
        public MethodInfo OnCompletedMethod { get; private set; }

        [CanBeNull]
        public MethodInfo UnsafeOnCompletedMethod { get; private set; }

        public static AwaiterInfo TryCreate(TypeInfo type)
        {
            Debug.Assert(type != null);

            if (!NotifyCompletionTypeInfo.IsAssignableFrom(type)) return null;

            var isCompletedMethod = type
                .GetRuntimeProperties()
                .SingleOrDefault(z => 
                    z.Name == nameof(TaskAwaiter.IsCompleted) &&
                    z.PropertyType == typeof(bool) && 
                    z.GetIndexParameters().Length == 0)
                ?.GetMethod;
            if (isCompletedMethod == null) return null;

            var getResultMethod = type
                .GetRuntimeMethods()
                .SingleOrDefault(z => z.Name == nameof(TaskAwaiter.GetResult) && z.GetParameters().Length == 0);
            if (getResultMethod == null) return null;

            var info = new AwaiterInfo(type)
            {
                OnCompletedMethod = type
                    .GetRuntimeInterfaceMap(typeof(INotifyCompletion)).TargetMethods
                    .Single(z => z.Name == nameof(INotifyCompletion.OnCompleted)),
                IsCompletedMethod = isCompletedMethod,
                GetResultMethod = getResultMethod
            };

            if (CriticalNotifyCompletionTypeInfo.IsAssignableFrom(type))
            {
                info.UnsafeOnCompletedMethod = type
                    .GetRuntimeInterfaceMap(typeof(ICriticalNotifyCompletion)).TargetMethods
                    .SingleOrDefault(z => z.Name == nameof(ICriticalNotifyCompletion.UnsafeOnCompleted));
            }

            return info;
        }
    }
}