using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pickaxe.Helpers;
using Pickaxe.Models;

public class InstanceLoader : ListModel<Instance> {
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