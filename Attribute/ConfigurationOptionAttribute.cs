using System;

namespace Qonfig
{
    /// <summary>
    /// Attribute for configuration enum items
    /// </summary>
    public class ConfigurationOptionAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        public bool NonPersistent { get; set; }
        /// <summary>
        /// Name of value, may contain ':' or '/' or '\' for grouping, or use 'Path' instead
        /// e.g. 'Style' or 'Application:Style' (same as Name='Style' and Path='Application')
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Default value, Type or DefaultValue must be set
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// Path of option, may contain ':' or '/' or '\' for grouping e.g. 'Application:Style'
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Value type, Type or DefaultValue must be set
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Constructor with value type (if you want empty value)
        /// </summary>
        /// <param name="name">Name of value, may contain ':' or '/' or '\' for grouping, or use 'Path' instead
        /// e.g. 'Style' or 'Application:Style' (same as Name='Style' and Path='Application')</param>
        /// <param name="type">Value type, Type or DefaultValue must be set</param>
        public ConfigurationOptionAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Constructor with defaultValue
        /// </summary>
        /// <param name="name">Name of value, may contain ':' or '/' or '\' for grouping, or use 'Path' instead
        /// e.g. 'Style' or 'Application:Style' (same as Name='Style' and Path='Application')</param>
        /// <param name="defaultValue">Default value, Type or DefaultValue must be set</param>
        public ConfigurationOptionAttribute(string name, object defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}