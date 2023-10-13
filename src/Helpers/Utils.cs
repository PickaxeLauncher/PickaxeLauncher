using System;
using System.IO;
using Nickvision.Aura;

namespace Pickaxe.Helpers;

public static class Utils {
    public static string GetAppFolder() {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Aura.Active.AppInfo.ID);
    }

    public static string GetAppFolder(string path) {
        var f = GetAppFolder();
        return Path.Combine(f, path);
    }
}