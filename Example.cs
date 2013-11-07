using System;
using Qonfig.Adapters;
using Qonfig.Extensions;

namespace Qonfig
{
    /// <summary>
    /// Example, create configuration explicitly and get/set values
    /// </summary>
    class ExampleExplicit
    {
        public enum ConfigOption
        {
            [ConfigurationOption("Application:Style", 0)]
            AppStyle,
            [ConfigurationOption("Application:Localization:Language", "us-US")]
            AppLanguage,
            [ConfigurationOption("Application:LastUpdate", typeof(DateTime))]
            Backup,
            // OR, if you prefer
            [ConfigurationOption("Language2", "us-US", Path = "Application:Localization")]
            AppLanguage2
        }
        public ExampleExplicit()
        {
            // hand over the enum type for uhm.. expliciticity..?
            Configurator.Init<ConfigOption>("Company:Product", typeof(RegistryAdapter), true);

            // explicit get
            Console.WriteLine(ConfigOption.AppStyle.GetConfigurationValue<int>());
            // OR explicit-set and dynamic get (if you're in a referenced project f.e.)
            Console.WriteLine(Configurator.Get<string>("Application:Localization:Language"));

            // explicit set
            ConfigOption.Backup.SetConfigurationValue(2);
        }
    }

    /// <summary>
    /// Example, create configuration dynamically and get/set values
    /// </summary>
    class ExampleDynamic
    {
        public ExampleDynamic()
        {
            // none-explicit initialization
            Configurator.Init("Company:Product");

            // dynamically add config-options
            Configurator.Library.Add("Application:Style", 0);
            Configurator.Library.Add("Application:Localization:Language");

            // get dynamically
            Console.WriteLine(Configurator.Get<int>("Application:Style"));
            Console.WriteLine(Configurator.Get<string>("Application:Localization:Language"));

            // set dynamically
            Configurator.Set("Application:Localization:Language", "de-DE");
        }
    }
}