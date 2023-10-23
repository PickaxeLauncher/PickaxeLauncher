using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Installer.FabricMC;
using CmlLib.Core.Installer.Forge.Versions;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using CmlLib.Core.VersionMetadata;
using Pickaxe.Models;

namespace Pickaxe.Controllers;

public class NewInstanceController {
    private readonly InstanceLoader _loader;
    private Instance _instance { get; set; } = new();
    private MVersionMetadata _version { get; set; }
    public NewInstanceController(InstanceLoader loader) {
        _loader = loader;
    }


    public void SetName(string name) => _instance.Name = name;
    public void SetVersion(MVersionMetadata version) {
        _instance.Version = version.Name;
        _version = version;
    }

    public void SetIcon(GdkPixbuf.Pixbuf icon) {
        var outFile = Path.Combine(_instance.Path, "icon.png");
        icon.Savev(outFile, "png", null, null);
        _instance.Icon = outFile;
    }

    public bool HasName() => _instance.Name != null;
    public bool HasVersion() => _instance.Version != null;

    public async Task<MVersionCollection> GetVersions() {
        return await new MojangVersionLoader().GetVersionMetadatasAsync();
    }


    public event EventHandler OnDoneInstalling;
    public event EventHandler<double> OnProgressChanged;
    public event EventHandler<string> OnStatusChanged;

    public void Install() {
        var launcher = new CMLauncher(_instance.MinecraftPath);
        launcher.FileChanged += (s) => {
            OnStatusChanged.Invoke(this, s.FileKind switch {
                CmlLib.Core.Downloader.MFile.Library => "Downloading Libraries",
                CmlLib.Core.Downloader.MFile.Resource => "Downloading Resources",
                CmlLib.Core.Downloader.MFile.Minecraft => "Downloading Minecraft",
                CmlLib.Core.Downloader.MFile.Runtime => "Downloading Runtime",
                CmlLib.Core.Downloader.MFile.Others => "Downloading Others",
                _ => "Downloading"
            });
            var progress = (double)s.ProgressedFileCount / s.TotalFileCount;
            OnProgressChanged.Invoke(this, progress);
        };
        Task.Run(async () => {
            var version = await _version.GetVersionAsync();
            await launcher.CheckAndDownloadAsync(version);
            _loader.Add(_instance);
            await _instance.Save();
            OnDoneInstalling.Invoke(this, null);
        });
    }
}