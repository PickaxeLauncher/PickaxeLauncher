using Nickvision.Aura;

namespace Pickaxe.Models;

/// <summary>
/// A model for the configuration of the application
/// </summary>
public class Configuration : ConfigurationBase
{
    /// <summary>
    /// Constructs a Configuration
    /// </summary>
    public Configuration()    {
    }

    /// <summary>
    /// Gets the singleton object
    /// </summary>
    internal static Configuration Current => (Configuration)Aura.Active.ConfigFiles["config"];
}
