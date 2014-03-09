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
        /// A local cache of the instance.
        /// </summary>
        private static readonly Lazy<T> LazyInstance = AppDomain.CurrentDomain.IsDefaultAppDomain() ? CreateOnDefaultAppDomain() : CreateOnOtherAppDomain();

        /// <summary>
        /// Returns a lazy type that creates the instance (if necessary) and saves it in the domain data. This method must only be called from the default AppDomain.
        /// </summary>
        private static Lazy<T> CreateOnDefaultAppDomain()
        {
            return new Lazy<T>(() =>
            {
                var ret = new T();
                AppDomain.CurrentDomain.SetData(Name, ret);
                return ret;
            });
        }

        /// <summary>
        /// Returns a lazy type that calls into the default domain to create the instance and retrieves a proxy into the current domain.
        /// </summary>
        private static Lazy<T> CreateOnOtherAppDomain()
        {
            return new Lazy<T>(() =>
            {
                var defaultAppDomain = AppDomainHelper.DefaultAppDomain;
                defaultAppDomain.DoCallBack(CreateCallback);
                return (T)defaultAppDomain.GetData(Name);
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
        /// Gets the process-wide instance. If the current domain is not the default AppDomain, this property returns a proxy to the actual instance.
        /// </summary>
        public static T Instance { get { return LazyInstance.Value; } }
    }
}
