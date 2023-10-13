using Nickvision.Aura;
using Pickaxe.Models;

namespace Pickaxe.Controllers;

/// <summary>
/// A controller for a PreferencesView
/// </summary>
public class PreferencesController {
    /// <summary>
    /// Constructs a PreferencesViewController
    /// </summary>
    internal PreferencesController() {

    }

    /// <summary>
    /// Saves the configuration to disk
    /// </summary>
    public void SaveConfiguration() => Aura.Active.SaveConfig("config");
}
