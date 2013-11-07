using System;
using Microsoft.Win32;

namespace Qonfig.Adapters
{
    /// <summary>
    /// Registry storage adapter
    /// </summary>
    class RegistryAdapter : ConfigurationAdapter
    {
        /// <summary>
        /// Default-Constructor
        /// </summary>
        /// <param name="forceLocalStorage">Store data locally (e.g. localappdata on filesystem or CurrentUser on registry)</param>
        /// <param name="path">Path of option (e.g. 'Company\\Product')</param>
        public RegistryAdapter(bool forceLocalStorage, string path)
            : base(forceLocalStorage, path)
        {
        }

        /// <summary>
        /// Registry-agnostc get
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <returns>Option-Value as object</returns>
        public override string Get(string name)
        {
            var registryValue = String.Empty;

            var value = GetLocalKey(ref name).GetValue(name);
            if (value != null)
                registryValue = value.ToString();

            return registryValue;
        }

        /// <summary>
        /// Registry-agnostic set
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <param name="value">Value of option</param>
        public override void Set(string name, string value)
        {
            GetLocalKey(ref name).SetValue(name, value);
        }

        /// <summary>
        /// Deletes an option from storage
        /// </summary>
        /// <param name="name">Name of item</param>
        public override void Delete(string name)
        {
            var localKey = GetLocalKey(ref name);
            localKey.DeleteValue(name);
        }

        /// <summary>
        /// Clears an option from storage, so delete value and let it return default
        /// </summary>
        /// <param name="name">Name of item</param>
        public override void Clear(string name)
        {
            GetLocalKey(ref name).SetValue(name, String.Empty);
        }

        /// <summary>
        /// Gets the local key for get/set operations
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <returns></returns>
        public RegistryKey GetLocalKey(ref string name)
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
            {
                if (IsElevated && !ForceLocalStorage)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            }
            else
            {
                if (IsElevated && !ForceLocalStorage)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            }
            localKey = localKey.CreateSubKey(String.Format(@"Software\{0}", Path));

            if (!name.Contains("\\")) return localKey;

            var path = name.Substring(0, name.LastIndexOf("\\", StringComparison.Ordinal));
            name = name.Substring(path.Length + 1, name.Length - path.Length - 1);
            localKey = localKey.CreateSubKey(path);
            return localKey;
        }
    }
}
