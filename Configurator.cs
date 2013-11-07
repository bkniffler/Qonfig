using System;
using System.Linq;
using Qonfig.Adapters;
using Qonfig.Dictionaries;
using Qonfig.Extensions;

namespace Qonfig
{
    /// <summary>
    /// Saves configuration options to registry.
    /// Stores options within CurrentUser or LocalMachine according to privileges.
    /// Options are stored as empty values if default (not actively set), but return default values
    /// </summary>
    public static class Configurator
    {
        /// <summary>
        /// Initialize Configuration
        /// </summary>
        /// <param name="path">Relative path of configuration, e.g. 'Company' or 'Company:Product' (feel free to use '/' or '\\' instead of ':')</param>
        /// <param name="adapterType">AdapterType, must derive from ConfigurationAdapter, defaults to RegitryAdapter if null</param>
        /// <param name="forceLocalStorage">Force to use local storage (e.g. CurrentUser in registry or LocalAppData on filestorage)</param>
        public static void Init(string path, Type adapterType = null, bool forceLocalStorage = false)
        {
            if (path == String.Empty) throw new Exception("Empty value not allowed on path!");
            if (adapterType == null) adapterType = typeof(RegistryAdapter);
            if (!typeof(ConfigurationAdapter).IsAssignableFrom(adapterType)) throw new Exception("AdapterType must implement ConfigurationAdapter!");
            Library.ItemAdded += (sender, item) => Init(item.Key);
            _init = true;
            _forceLocalStorage = forceLocalStorage;
            _path = path.Replace("/", "\\").Replace(":", "\\");

            _adapter = (ConfigurationAdapter)Activator.CreateInstance(adapterType, _forceLocalStorage, _path);
        }

        /// <summary>
        /// Initialize Configuration with options-enum T
        /// </summary>
        /// <typeparam name="T">Enum Type of options-enum, don't forget to implement ConfigurationOptionAttribute on each enum item!<</typeparam>
        /// <param name="path">Relative path of configuration, e.g. 'Company' or 'Company:Product' (feel free to use '/' or '\\' instead of ':')</param>
        /// <param name="adapterType">AdapterType, must derive from ConfigurationAdapter, defaults to RegitryAdapter if null</param>
        /// <param name="forceLocalStorage">Force to use local storage (e.g. CurrentUser in registry or LocalAppData on filestorage)</param>
        public static void Init<T>(string path, Type adapterType = null, bool forceLocalStorage = false)
        {
            Init(path, adapterType, forceLocalStorage);

            var items = Enum.GetValues(typeof(T));
            foreach (var item in items)
                ((Enum)item).InitConfigurationValue();
        }

        #region Private Fields
        /// <summary>
        /// Is initialized?
        /// </summary>
        private static bool _init;
        /// <summary>
        /// Adapter
        /// </summary>
        private static ConfigurationAdapter _adapter;
        /// <summary>
        /// e.g. 'Company' or 'Company\\Product'
        /// </summary>
        private static string _path = String.Empty;
        /// <summary>
        /// Forces the Library to save within CurrentUser registry branch, instead of automatically choosing CurrentUser or LocalMachine according to privileges
        /// </summary>
        private static bool _forceLocalStorage;
        /// <summary>
        /// Collection of configuration options with values
        /// </summary>
        private static ObservableDictionary _library = new ObservableDictionary(); 
        #endregion

        #region Public Fields
        /// <summary>
        /// Dictionary containing all configuration options and default values, e.g. "url":"http://quambo.net" or "date":"20.01.1988"
        /// </summary>
        public static ObservableDictionary Library
        {
            get
            {
                return _library;
            }
            set
            {
                _library = value;
                foreach (var item in Library)
                    Init(item.Key);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Is a config option actively set? False if default.
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <returns></returns>
        public static bool IsSet(string name)
        {
            name = CleanAndCheckName(name);

            object value = _adapter.Get(name);

            if (value == null || String.IsNullOrEmpty(value.ToString()))
                return false;
            return true;
        }

        /// <summary>
        /// Get an option value
        /// </summary>
        /// <typeparam name="TVal">Type of option</typeparam>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <returns></returns>
        public static TVal Get<TVal>(string name)
        {
            object value;
            if (Library.IsNonPersistent(name))
                value = Library[name];
            else
            {
                name = CleanAndCheckName(name);
                value = _adapter.Get(name);
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                    value = Library[name];
            }
            return value.ChangeType<TVal>();
        }

        /// <summary>
        /// Set an option value
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <param name="value">Value of option</param>
        public static void Set(string name, object value)
        {
            if (Library.IsNonPersistent(name))
                Library[name] = value;
            else
            {
                name = CleanAndCheckName(name);
                value = value != null ? value.ToString() : String.Empty;
                _adapter.Set(name, (String)value);
            }
        }

        /// <summary>
        /// Deletes a saved option
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        public static void Delete(string name)
        {
            if (!_init) throw new Exception("Initialize Configuration first!");
            if (_adapter == null) throw new Exception("No configuration adapter found!");
            if (Library.IsNonPersistent(name))
                return;
            _adapter.Delete(name);
        }

        /// <summary>
        /// Deletes all saved options
        /// </summary>
        public static void DeleteAll()
        {
            if (!_init) throw new Exception("Initialize Configuration first!");
            if (_adapter == null) throw new Exception("No configuration adapter found!");
            foreach (var item in Library.Where(p => !Library.IsNonPersistent(p.Key)))
                _adapter.Delete(item.Key);
        }

        /// <summary>
        /// Clears a saved options, replacing its value with an empty value, so default value is returned in future
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        public static void Clear(string name)
        {
            if (!_init) throw new Exception("Initialize Configuration first!");
            if (_adapter == null) throw new Exception("No configuration adapter found!");
            if (Library.IsNonPersistent(name))
                return;
            _adapter.Clear(name);
        }

        /// <summary>
        /// Clears all saved options, replacing its value with an empty value, so default value is returned in future
        /// </summary>
        public static void ClearAll()
        {
            if (!_init) throw new Exception("Initialize Configuration first!");
            if (_adapter == null) throw new Exception("No configuration adapter found!");
            foreach (var item in Library.Where(p => !Library.IsNonPersistent(p.Key)))
                _adapter.Clear(item.Key);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Initializes an option
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        private static void Init(string name)
        {
            if (Library.IsNonPersistent(name))
                return;
            name = CleanAndCheckName(name);
            if (IsSet(name)) return;
            Set(name, String.Empty);
        } 

        /// <summary>
        /// Checks if everything is OK and returns a clean name, with ':' and '/' replaced to '\'
        /// </summary>
        /// <param name="name">Name of option, may contain '\' or ':' or '/' for grouping, e.g. 'NETWORK:CLIENTIP'</param>
        /// <returns></returns>
        private static string CleanAndCheckName(string name)
        {
            if (!_init) throw new Exception("Initialize Configuration first!");
            if (_adapter == null) throw new Exception("No configuration adapter found!");

            if (name.Contains("/")) name = name.Replace("/", "\\");
            if (name.Contains(":")) name = name.Replace(":", "\\");
            if (!Library.ContainsKey(name)) throw new Exception(String.Format("Could not find option with the name {0} in configuration library", name));
            return name;
        }

        #endregion
    }
}