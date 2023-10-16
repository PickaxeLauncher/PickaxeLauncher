using System.Collections.Generic;
using System.Threading.Tasks;
using Modrinth;
using Pickaxe.Models;

namespace Pickaxe.Providers;

public class ModrinthModProvider : IModProvider {
    private ModrinthClient _client;

    public ModrinthModProvider() {
        var userAgent = new ModrinthClientConfig {
            UserAgent = "PickaxeLauncher"
        };
        _client = new ModrinthClient(userAgent);
    }

    public async Task<List<ModrinthMod>> Search(string name) {
        var search = await _client.Project.SearchAsync(name);
        var mods = new List<ModrinthMod>();
        foreach (var project in search.Hits) {
            var mod = new ModrinthMod {
                Name = project.Title
            };
            mods.Add(mod);
        }
        return mods;
    }
}


public class ModrinthMod : IMod {
    public string Name { get; set; }
    public ModSource Source => ModSource.Modrinth;
}