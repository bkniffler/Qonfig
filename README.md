Qonfig
======

**Very Simple and expandable configuration framework.**
Lets take a look at how easy it could possibly get to create a configuration context for your application.

### Specific
```c#
public enum ConfigOption
{
    [ConfigurationOption("App:Style", "nice-style")]
    AppStyle
}
class Program
{
    public void Main()
    {
        Configurator.Init<ConfigOption>("Company:Product", typeof(RegistryAdapter));
        ConfigOption.AppStyle.GetConfigurationValue<string>();
        ConfigOption.AppStyle.SetConfigurationValue("nicer-style");
    }
}
```

So, whats going on here? First, we define the configuration model. Take a look at the `ConfigOption` enum. It contains an option, decorated with a special attribute that defines the path and name (use ':', '/' or '\' for separation) and the default value.

Then, we execute the Main function. First thing is to initialize the static `Configurator` class. As we want to use specific configuration options, we pass the `ConfigOption` enum type as `<T>` and the configuration storage path (again, choose the separator) as a string. Optionally, set the adapter type or get the default one (`RegistryAdapter`).

This will create an entry to your registry, under "LocalMachine or CurrentUser\Software\Company\Product\App" named "Style" with an empty value.

Why the value is empty you ask? The answer is, why store it, if it's default anyways? This way, if you decide to change the default value, the old default value is safely ignored on machines that have executed your old code before. Why create it you ask? For the sake of letting them users know, what options there is to set! 

Wether to use LocalMachine or CurrentUser is left to be decided by the app, depending on the privileges it has. A behavior that may be customized by setting `forceLocalStorage` in the constructor to be true:
```c#
Configurator.Init<ConfigOption>("Company:Product", typeof(RegistryAdapter), true); 
```

Next, use the enum extensions `GetConfigurationValue<T>` and `SetConfigurationValue` to get the value of a specific type or to set it. 

Thats it for explicit usage of the library. So lets get on with dynamic usage!

### Dynamic
```c#
class Program
{
    public void Main()
    {
        Configurator.Init("Company:Product");
        Configurator.Library.Add("App:Style", "nice-style");
        Configurator.Get<string>("App:Style");
        Configurator.Set("App:Style", "nicer-style");
    }
}   
```
Ok, initialize the `Configurator` with a storage path and dynamically add as much configuration items to the `Library` as you wish. Define the path and name as well as a default value, if you wish.

### Non-Defaulted

What if you don't want those default values? You wished you could just add options without specifying any value? Easy, just set the attributes' typeof or dynamically add the option without anything specified but the path/name.

```c#
public enum ConfigOption
{
    [ConfigurationOption("Application:Style", typeof(string))]
    AppStyle
} 
// OR dynamically
Configurator.Library.Add("App:Style");   
```

### Non-Persistent
You'd like to use configuration options that shall not be modified by any user? Go on, define the `NonPersistent` property and assign it the `true` value! Or add the option using the `Library.AddNonPersistent` method if you're on the dynamic side.
```c#
public enum ConfigOption
{
    [ConfigurationOption("Application:Style", typeof(string), NonPersistent = true)]
    AppStyle
}  
// OR dynamically
Configurator.Library.AddNonPersistent("App:Style", "nice-style");    
```
And thats it, nice and easy persistency control!

### Storage
Choices are always good, so how do I store my data? Just specify an adapter or go and write your own (just implement the abstract `ConfigurationAdapter` class and you're set)!
```c#
Configurator.Init<ConfigOption>("Company:Product", typeof(RegistryAdapter)); 
```

As said, you can force the local storage to be used by specifying 'true' within the constructor. Imagine the LocalAppData to always be used (File-Storage) or the CurrentUser in the registry.
