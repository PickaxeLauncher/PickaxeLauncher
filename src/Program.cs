using Pickaxe.Views;
using Pickaxe.Controllers;
using Pickaxe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nickvision.Aura;
using Pickaxe.Helpers;
using System.Threading.Tasks;
using Pickaxe.Services;
using CmlLib.Core;

namespace Pickaxe;

public partial class Program : GetTextMixin {
    private readonly Adw.Application _application;
    private readonly MainWindowController _mainWindowController;
    private MainWindow _mainWindow;

    public Program(string[] args) {
        Aura.Init("dev.bedsteler20.Pickaxe", "Nickvision Application");
        Aura.Active.SetConfig<Configuration>("config");
        Aura.Active.AppInfo.Version = "2023.9.0-next";
        Aura.Active.AppInfo.ShortName = _("Application");
        Aura.Active.AppInfo.Description = _("Create new Nickvision applications");
        Aura.Active.AppInfo.SourceRepo = new Uri("https://github.com/NickvisionApps/Application");
        Aura.Active.AppInfo.IssueTracker =
            new Uri("https://github.com/NickvisionApps/Application/issues/new");
        Aura.Active.AppInfo.SupportUrl =
            new Uri("https://github.com/NickvisionApps/Application/discussions");
        Aura.Active.AppInfo.ExtraLinks[_("Matrix Chat")] =
            new Uri("https://matrix.to/#/#nickvision:matrix.org");
        Aura.Active.AppInfo.Developers[_("Nicholas Logozzo")] =
            new Uri("https://github.com/nlogozzo");
        Aura.Active.AppInfo.Developers[_("Contributors on GitHub ❤️")] =
            new Uri("https://github.com/NickvisionApps/Application/graphs/contributors");
        Aura.Active.AppInfo.Designers[_("Nicholas Logozzo")] =
            new Uri("https://github.com/nlogozzo");
        Aura.Active.AppInfo.Designers[_("Fyodor Sobolev")] = new Uri("https://github.com/fsobolev");
        Aura.Active.AppInfo.Designers[_("DaPigGuy")] = new Uri("https://github.com/DaPigGuy");
        Aura.Active.AppInfo.Artists[_("David Lapshin")] = new Uri("https://github.com/daudix-UFO");
        Aura.Active.AppInfo.TranslatorCredits = _("translator-credits");
        _application = Adw.Application.New(Aura.Active.AppInfo.ID, Gio.ApplicationFlags.NonUnique);
        _mainWindow = null;
        _mainWindowController = new MainWindowController();
        _application.OnActivate += OnActivate;

        if (File.Exists(
                Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) +
                "/dev.bedsteler20.Pickaxe.gresource")) {
            //Load file from program directory, required for `dotnet run`
            Gio.Functions.ResourcesRegister(Gio.Functions.ResourceLoad(
                Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) +
                "/dev.bedsteler20.Pickaxe.gresource"));
        } else {
            var prefixes = new List<string> {
                Directory.GetParent(Directory
                        .GetParent(Path.GetFullPath(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
                        .FullName)
                    .FullName,
                Directory.GetParent(
                    Path.GetFullPath(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))).FullName,
                "/usr"
            };
            foreach (var prefix in prefixes) {
                if (File.Exists(prefix +
                                "/share/dev.bedsteler20.Pickaxe/dev.bedsteler20.Pickaxe.gresource")) {
                    Gio.Functions.ResourcesRegister(Gio.Functions.ResourceLoad(
                        Path.GetFullPath(prefix +
                                         "/share/dev.bedsteler20.Pickaxe/dev.bedsteler20.Pickaxe.gresource")));
                    break;
                }
            }
        }
    }

    public static int Main(string[] args) => new Program(args).Run();

    public int Run(string[] args = null) {
        try {
            return _application.RunWithSynchronizationContext(args);
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"\n\n{ex.StackTrace}");
            return -1;
        }
    }

    private async Task InitulizeServices() {
        Injector.InstallService(new CMLauncher(Utils.GetAppFolder()));
    } 

    private async void OnActivate(Gio.Application sedner, EventArgs e) {
        //Main Window
        _mainWindow = new MainWindow(_mainWindowController, _application);
        await _mainWindow.StartAsync();
    }
}