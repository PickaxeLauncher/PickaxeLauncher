using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nickvision.Aura;

namespace Pickaxe.Helpers;

public static class Utils {
    public static string GetAppFolder() {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Aura.Active.AppInfo.ID);
    }

    public static string GetAppFolder(params string[] paths) {
        var f = GetAppFolder();
        return Path.Combine(f, Path.Combine(paths));
    }

    public static string GetCacheFolder() {
        var xdg_cache = Environment.GetEnvironmentVariable("XDG_CACHE_HOME") ??
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            ".cache");
        return Path.Combine(xdg_cache, Aura.Active.AppInfo.ID);
    }

    public static string GetCacheFolder(params string[] paths) {
        var f = GetCacheFolder();
        return Path.Combine(f, Path.Combine(paths));
    }

    public static T LoadJson<T>(string path) {
        if (!File.Exists(path)) {
            return default;
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json);
    }
}