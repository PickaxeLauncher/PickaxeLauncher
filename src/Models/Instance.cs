using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using Pickaxe.Helpers;
using Pickaxe.Helpers.Extension;

namespace Pickaxe.Models;

public class Instance {
    private const string INSTANCE_DIR_NAME = "Instances";
    private const string INSTANCE_FILE_NAME = "Instance.json";
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Version { get; set; }

    public string Path => Utils.GetAppFolder(INSTANCE_DIR_NAME, Name);

    public MinecraftPath MinecraftPath =>
        new MinecraftPath(Utils.GetAppFolder(INSTANCE_DIR_NAME, Name, "minecraft"));

    public async static Task<Instance> Load(string name) {
        var path = Utils.GetAppFolder(INSTANCE_DIR_NAME, name, INSTANCE_FILE_NAME);
        if (!File.Exists(path)) {
            return default;
        }

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<Instance>(json);
    }

    public async Task Save() {
        var path = Utils.GetAppFolder(INSTANCE_DIR_NAME, Name, INSTANCE_FILE_NAME);
        var json = JsonSerializer.Serialize(this);
        await File.WriteAllTextAsync(path, json);
    }
}