using System;

namespace DomainAwareSingleton
{
    /// <summary>
    /// A domain-aware singleton. Only one instance of <typeparamref name="T"/> will exist, belonging to the default AppDomain. All members of this type are threadsafe.
    /// </summary>
    /// <typeparam name="T">The type of instance managed by this singleton.</typeparam>
    public static class Singleton<T> where T : MarshalByRefObject, new()
    {
        /// <summary>
        /// Gets the domain data key for this type. This property may be called from any AppDomain, and will return the same value regardless of AppDomain.
        /// </summary>
        private static string Name
        {
            get { return "BA9A49C7E9364060AC4E4DDDBF465684." + typeof(T).FullName; }
        }

        /// <summary>
        /// A local cache of the instance wrapper.
        /// </summary>
        private static readonly Lazy<Wrapper> LazyInstance = AppDomain.CurrentDomain.IsDefaultAppDomain() ? CreateOnDefaultAppDomain() : CreateOnOtherAppDomain();

        /// <summary>
        /// A local cache of the instance.
        /// </summary>
        private static readonly Lazy<T> CachedLazyInstance = new Lazy<T>(() => Instance);

        /// <summary>
        /// Returns a lazy that creates the instance (if necessary) and saves it in the domain data. This method must only be called from the default AppDomain.
        /// </summary>
        private static Lazy<Wrapper> CreateOnDefaultAppDomain()
        {
            return new Lazy<Wrapper>(() =>
            {
                var ret = new Wrapper { WrappedInstance = new T() };
                AppDomain.CurrentDomain.SetData(Name, ret);
                return ret;
            });
        }

        /// <summary>
        /// Returns a lazy that calls into the default domain to create the instance and retrieves a proxy into the current domain.
        /// </summary>
        private static Lazy<Wrapper> CreateOnOtherAppDomain()
        {
            return new Lazy<Wrapper>(() =>
            {
                var defaultAppDomain = AppDomainHelper.DefaultAppDomain;
                var ret = defaultAppDomain.GetData(Name) as Wrapper;
                if (ret != null)
                    return ret;
                defaultAppDomain.DoCallBack(CreateCallback);
                return (Wrapper)defaultAppDomain.GetData(Name);
            });
        }

        /// <summary>
        /// Ensures the instance is created (and saved in the domain data). This method must only be called on the default AppDomain.
        /// </summary>
        private static void CreateCallback()
        {
            var _ = LazyInstance.Value;
        }

        /// <summary>
        /// Gets the process-wide instance. If the current domain is not the default AppDomain, this property returns a new proxy to the actual instance.
        /// </summary>
        public static T Instance { get { return LazyInstance.Value.WrappedInstance; } }

        /// <summary>
        /// Gets the process-wide instance. If the current domain is not the default AppDomain, this property returns a cached proxy to the actual instance. It is your responsibility to ensure that the cached proxy does not time out; if you don't know what this means, use <see cref="Instance"/> instead.
        /// </summary>
        public static T CachedInstance { get { return CachedLazyInstance.Value; } }

        private sealed class Wrapper : MarshalByRefObject
        {
            public override object InitializeLifetimeService()
            {
                return null;
            }

            public T WrappedInstance { get; set; }
        }
    }
}
