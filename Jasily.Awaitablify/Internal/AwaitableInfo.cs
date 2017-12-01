using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Jasily.Awaitablify.Internal
{
    internal class AwaitableInfo
    {
        private AwaitableInfo([NotNull] Type awaitableType, [NotNull] MethodInfo getAwaiterMethod, [NotNull] AwaiterInfo awaiterInfo)
        {
            this.AwaitableType = awaitableType ?? throw new ArgumentNullException(nameof(awaitableType));
            this.GetAwaiterMethod = getAwaiterMethod ?? throw new ArgumentNullException(nameof(getAwaiterMethod));
            this.AwaiterInfo = awaiterInfo ?? throw new ArgumentNullException(nameof(awaiterInfo));
        }

        [NotNull]
        public Type AwaitableType { get; }

        [NotNull]
        public MethodInfo GetAwaiterMethod { get; }

        [NotNull]
        public AwaiterInfo AwaiterInfo { get; }

        /// <summary>
        /// whatever value is null or not, still need check whether can resolve <see cref="AwaitableInfo"/> from method return value.
        /// </summary>
        [CanBeNull]
        public MethodInfo ConfigureAwaitMethod { get; private set; }

        [NotNull]
        public Type ResultType => this.AwaiterInfo.GetResultMethod.ReturnType;

        public static AwaitableInfo TryCreate([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var runtimeMethods = type.GetRuntimeMethods().ToArray();

            var getAwaiterMethod = runtimeMethods.FirstOrDefault(z =>
                z.Name == nameof(Task.GetAwaiter) &&
                z.ReturnType != typeof(void) &&
                z.GetParameters().Length == 0);
            if (getAwaiterMethod == null) return null;

            var awaiterType = getAwaiterMethod.ReturnType;

            var awaiterInfo = AwaiterInfo.TryCreate(awaiterType.GetTypeInfo());
            if (awaiterInfo == null) return null;

            var info = new AwaitableInfo(type, getAwaiterMethod, awaiterInfo)
            {
                ConfigureAwaitMethod = runtimeMethods.FirstOrDefault(z =>
                    z.Name == nameof(Task.ConfigureAwait) &&
                    z.ReturnType != typeof(void) &&
                    z.GetParameters().Length == 1 &&
                    z.GetParameters()[0].ParameterType == typeof(bool))
            };

            return info;
        }
    }
}