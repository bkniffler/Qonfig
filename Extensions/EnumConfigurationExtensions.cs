using System;

namespace Qonfig.Extensions
{
    public static class EnumConfigurationExtensions
    {
        /// <summary>
        /// Enum extension, checks for [Description("")] attribute and returns its name field
        /// </summary>
        /// <param name="item">Enum value to check</param>
        /// <returns>Name of item</returns>
        public static string GetConfigurationName(this Enum item)
        {
            var type = item.GetType();
            var fieldInfo = type.GetField(item.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(ConfigurationOptionAttribute), false) as ConfigurationOptionAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                var attrib = attribs[0];
                if (String.IsNullOrEmpty(attrib.Path)) return attrib.Name;
                return attrib.Path + "\\" + attrib.Name;
            }
            throw new Exception("'ConfigurationOptionAttribute' not found on " + item);
        }

        /// <summary>
        /// Enum extension, checks for [Description("")] attribute and returns its default-value or its type
        /// </summary>
        /// <param name="item">Enum value to check</param>
        /// <returns>Default-Value of item or type</returns>
        public static void InitConfigurationValue(this Enum item)
        {
            var type = item.GetType();
            var fieldInfo = type.GetField(item.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(ConfigurationOptionAttribute), false) as ConfigurationOptionAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                var attrib = attribs[0];
                var name = String.Empty;
                if (String.IsNullOrEmpty(attrib.Path)) name = attrib.Name;
                else name = attrib.Path + "\\" + attrib.Name;
                name = name.Replace(':', '\\').Replace('/', '\\');

                if (attrib.NonPersistent)
                    Configurator.Library.AddNonPersistent(name, attrib.DefaultValue);
                if (attrib.Type != null)
                    Configurator.Library.Add(name);
                else
                    Configurator.Library.Add(((Enum)item).GetConfigurationName().Replace(':', '\\').Replace('/', '\\'), attrib.DefaultValue);
                return;
            }
            throw new Exception("'ConfigurationOptionAttribute' not found on " + item);
        }

        /// <summary>
        /// Enum extension, returns configuration its value
        /// </summary>
        /// <param name="item">Enum value to check</param>
        /// <returns>Value of item</returns>
        public static T GetConfigurationValue<T>(this Enum item)
        {
            var type = item.GetType();
            var fieldInfo = type.GetField(item.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(ConfigurationOptionAttribute), false) as ConfigurationOptionAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                var attrib = attribs[0];
                string path;
                if (String.IsNullOrEmpty(attrib.Path)) path = attrib.Name;
                else path = attrib.Path + "\\" + attrib.Name;
                return Configurator.Get<T>(path);
            }
            throw new Exception("'ConfigurationOptionAttribute' not found on " + item);
        }

        /// <summary>
        /// Enum extension, checks for [Description("")] attribute and sets it value
        /// </summary>
        /// <param name="item">Enum value to check</param>
        /// <param name="value">New value</param>
        public static void SetConfigurationValue(this Enum item, object value)
        {
            var type = item.GetType();
            var fieldInfo = type.GetField(item.ToString());
            var attribs = fieldInfo.GetCustomAttributes(typeof(ConfigurationOptionAttribute), false) as ConfigurationOptionAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                var attrib = attribs[0];
                string path;
                if (String.IsNullOrEmpty(attrib.Path)) path = attrib.Name;
                else path = attrib.Path + "\\" + attrib.Name;
                Configurator.Set(path, value);
                return;
            }
            throw new Exception("'ConfigurationOptionAttribute' not found on " + item);
        }
    }
}