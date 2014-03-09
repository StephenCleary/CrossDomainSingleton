using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainAwareSingleton
{
    /// <summary>
    /// Provides utility methods related to the <see cref="AppDomain"/> type. All members of this type are threadsafe.
    /// </summary>
    public static class AppDomainHelper
    {
        /// <summary>
        /// The CorRuntimeHost, as an <see cref="ICorRuntimeHost"/>.
        /// </summary>
        private static readonly Lazy<ICorRuntimeHost> Host = new Lazy<ICorRuntimeHost>(
            () => (ICorRuntimeHost)Activator.CreateInstance(Type.GetTypeFromCLSID(Guid.Parse("CB2F6723-AB3A-11D2-9C40-00C04FA30A3E"))),
            LazyThreadSafetyMode.PublicationOnly); 

        /// <summary>
        /// The default AppDomain.
        /// </summary>
        private static readonly Lazy<AppDomain> LazyDefaultAppDomain = new Lazy<AppDomain>(() =>
        {
            object ret;
            Host.Value.GetDefaultDomain(out ret);
            return (AppDomain)ret;
        }, LazyThreadSafetyMode.PublicationOnly); 

        /// <summary>
        /// Gets the default AppDomain. This property caches the resulting value.
        /// </summary>
        public static AppDomain DefaultAppDomain
        {
            get { return LazyDefaultAppDomain.Value; }
        }

        /// <summary>
        /// Enumerates all currently-loaded AppDomains.
        /// </summary>
        public static IEnumerable<AppDomain> EnumerateLoadedAppDomains()
        {
            // http://devdale.blogspot.com/2007/10/getting-list-of-loaded-appdomains.html
            var host = Host.Value;
            IntPtr enumeration;
            host.EnumDomains(out enumeration);
            try
            {
                while (true)
                {
                    object domain = null;
                    host.NextDomain(enumeration, ref domain);
                    if (domain == null)
                        yield break;
                    yield return (AppDomain)domain;
                }
            }
            finally
            {
                host.CloseEnum(enumeration);
            }
        }

        [Guid("CB2F6722-AB3A-11D2-9C40-00C04FA30A3E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICorRuntimeHost
        {
            void _VtblGap_10();
            void GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)]out object appDomain);
            void EnumDomains(out IntPtr enumHandle);
            void NextDomain(IntPtr enumHandle, [MarshalAs(UnmanagedType.IUnknown)]ref object appDomain);
            void CloseEnum(IntPtr enumHandle);
        }
    }
}
