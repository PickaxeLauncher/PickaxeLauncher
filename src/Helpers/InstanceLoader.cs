using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pickaxe.Helpers;
using Pickaxe.Models;

public class InstanceLoader : List<Instance> {
    public InstanceLoader() {
        Add(new Instance {
            Name = "1.16.5",
            MinecraftVersion = new MinecraftVersion(),
            ModLoader = ModLoader.Fabric
        });
        Add(new Instance {
            Name = "1.17.1",
            MinecraftVersion = new MinecraftVersion(),
            ModLoader = ModLoader.Fabric
        });
        Add(new Instance {
            Name = "1.17.1",
            MinecraftVersion = new MinecraftVersion(),
            ModLoader = ModLoader.Vanilla
        });
    }

    public static string InstanceDir => Utils.GetAppFolder("Instances");

    public async Task StartupAsync() {
        if (!Directory.Exists(InstanceDir)) {
            Directory.CreateDirectory(InstanceDir);
        }
        foreach (var dir in Directory.GetDirectories(InstanceDir)) {
            var instance = await Instance.Load(Path.GetFileName(dir));
            if (instance != null) {
                Add(instance);
            }
        }
    }
}