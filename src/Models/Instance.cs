using System.Collections.Generic;

namespace Pickaxe.Models;

public class Instance {
    public string Name { get; set; }
    public string Path { get; set; }
    public MinecraftVersion MinecraftVersion { get; set; }
    public List<InstalledMod> InstalledMods { get; set; } = new();
    public IModLoader ModLoader { get; set; }
    
}