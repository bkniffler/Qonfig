using System.Security.Principal;

namespace Qonfig.Adapters
{
    /// <summary>
    /// Base Adapter
    /// </summary>
    abstract class ConfigurationAdapter
    {
        /// <summary>
        /// Default-Constructor
        /// </summary>
        /// <param name="forceLocalStorage">Store data locally (e.g. localappdata on filesystem or CurrentUser on registry)</param>
        /// <param name="path">Path of option (e.g. 'Company\\Product')</param>
        protected ConfigurationAdapter(bool forceLocalStorage, string path)
        {
            ForceLocalStorage = forceLocalStorage;
            Path = path;
        }

        /// <summary>
        /// Store data locally (e.g. localappdata on filesystem or CurrentUser on registry)
        /// </summary>
        internal readonly bool ForceLocalStorage;
        /// <summary>
        /// Path of option (e.g. 'Company\\Product')
        /// </summary>
        internal readonly string Path;
        /// <summary>
        /// Get value string by name
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <returns>String version of value</returns>
        public abstract string Get(string name);
        /// <summary>
        /// Set value string
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <param name="value">Value string of item</param>
        public abstract void Set(string name, string value);
        /// <summary>
        /// Deletes an option from storage
        /// </summary>
        /// <param name="name">Name of item</param>
        public abstract void Delete(string name);
        /// <summary>
        /// Clears an option from storage, so delete value and let it return default
        /// </summary>
        /// <param name="name">Name of item</param>
        public abstract void Clear(string name);

        /// <summary>
        /// Is this application elevated?
        /// </summary>
        internal bool IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}