﻿using Nickvision.Aura.Taskbar;
using Pickaxe.Helpers;
using Pickaxe.Controllers;
using System;
using System.Threading.Tasks;
using Nickvision.Aura;
using System.IO;
using Microsoft.Extensions.Logging;
using CmlLib.Core;
using Pickaxe.Models;

namespace Pickaxe.Views;

/// <summary>
/// The MainWindow for the application
/// </summary>
public partial class MainWindow : Adw.ApplicationWindow {
    [Gtk.Connect] private readonly Gtk.MenuButton _accountMenuButton;
    [Gtk.Connect] private readonly Gtk.ListBox _instanceList;
    [Gtk.Connect] private readonly Adw.NavigationSplitView _navView;
    [Gtk.Connect] private readonly Adw.Avatar _profilePic;
    [Gtk.Connect] private readonly Adw.WindowTitle _title;
    [Gtk.Connect] private readonly Gtk.Picture _icon;
    [Gtk.Connect] private readonly Gtk.Label _instanceNameLabel;

    private readonly Adw.Application _application;
    private readonly MainWindowController _controller;
    private Instance _selectedInstance;

    public MainWindow(MainWindowController controller, Adw.Application application) : this(
        Builder.FromFile("window.ui"), controller, application) {
    }

    private MainWindow(Gtk.Builder builder, MainWindowController controller,
        Adw.Application application) : base(builder.GetPointer("_root"), false) {
        _controller = controller;
        _application = application;
        SetDefaultSize(800, 600);
        SetTitle(Aura.Active.AppInfo.ShortName);
        SetIconName(Aura.Active.AppInfo.ID);
        if (Aura.Active.AppInfo.IsDevVersion) {
            AddCssClass("devel");
        }

        builder.Connect(this);
        _title.SetTitle(Aura.Active.AppInfo.ShortName);
        OnCloseRequest += OnCloseRequested;

        CreateAction("newInstance", NewInstance, new string[] { "<Ctrl>n" });
        CreateAction("preferences", Preferences, new string[] { "<Ctrl>comma" });
        CreateAction("keyboardShortcuts", KeyboardShortcuts, new string[] { "<Ctrl>question" });
        CreateAction("quit", Quit, new string[] { "<Ctrl>q" });
        CreateAction("about", About, new string[] { "F1" });
        CreateAction("addAccount", AddAccount);
        CreateAction("signOut", SignOut);
        _controller.AccountController.AccountChanged +=
            async (sender, args) => await SetupProfilePic();
    }

    private void OnInstanceSelected(Instance instance) {
        _selectedInstance = instance;
        _navView.SetShowContent(true);
        if (instance.Icon is not null) {
            _icon.SetFile(Gio.Functions.FileNewForPath(instance.Icon));
        } else {
            _icon.SetResource("/dev/bedsteler20/Pickaxe/minecraft.svg");
        }
        _title.SetSubtitle(instance.Name);
        _instanceNameLabel.SetLabel(instance.Name);
    }

    public new async Task Present() {
        BuildAccountSwitcher();
        BuildInstanceSidebar();
        await SetupProfilePic();
        base.Present();
    }

    private void BuildAccountSwitcher() {
        var menu = Gio.Menu.New();
        menu.AppendItem(Gio.MenuItem.New("Sign Out", "win.signOut"));
        // menu.AppendItem(Gio.MenuItem.New("Add Account", "win.addAccount"));
        _accountMenuButton.MenuModel = menu;
    }

    private void BuildInstanceSidebar() {
        _controller.InstanceLoader.Connect(_instanceList, instance => {
            Adw.ActionRow row = new() {
                Title = instance.Name
            };
            if (instance.Icon is not null) {
                row.AddPrefix(Gtk.Picture.NewForFile(Gio.Functions.FileNewForPath(instance.Icon)));
            } else {
                row.AddPrefix(Gtk.Picture.NewForResource("/dev/bedsteler20/Pickaxe/minecraft.svg"));
            }
            row.OnActivate += (sender, args) => OnInstanceSelected(instance);

            return row;
        });
    }

    private async Task SetupProfilePic() {
        try {
            var path = Utils.GetCacheFolder("heads");
            var name = _controller.AccountController.Username;
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            var headPath = Path.Combine(path, $"{name}.png");
            if (!File.Exists(headPath)) {
                var client = new System.Net.Http.HttpClient();
                var head = await client.GetByteArrayAsync($"https://mc-heads.net/avatar/{name}");
                await File.WriteAllBytesAsync(headPath, head);
            }

            var file = Gio.Functions.FileNewForPath(headPath);
            _profilePic.SetCustomImage(Gtk.IconPaintable.NewForFile(file, 200, 200));
        } catch (Exception e) {
            Console.WriteLine("Error setting profile pic", LogLevel.Error, "MainWindow", e);
        }
    }

    public async Task StartAsync() {
        _application.AddWindow(this);
        await _controller.StartupAsync();
        _controller.TaskbarItem =
            await TaskbarItem.ConnectLinuxAsync($"{Aura.Active.AppInfo.ID}.desktop");
        await Present();
    }

    private bool OnCloseRequested(Gtk.Window sender, EventArgs e) {
        _controller.Dispose();
        return false;
    }

    private void Preferences(Gio.SimpleAction sender, EventArgs e) {
        var preferencesDialog = new PreferencesDialog(_controller.CreatePreferencesViewController(),
            _application, this);
        preferencesDialog.Present();
    }

    private void AddAccount(Gio.SimpleAction sender, EventArgs e) {
        _ = _controller.AccountController.AddAccount();
    }

    private void SignOut(Gio.SimpleAction sender, EventArgs e) {
        _controller.AccountController.SignOut();
    }

    private void KeyboardShortcuts(Gio.SimpleAction sender, EventArgs e) {
        var builder = Builder.FromFile("shortcuts_dialog.ui");
        var shortcutsWindow = (Gtk.ShortcutsWindow)builder.GetObject("_shortcuts")!;
        shortcutsWindow.SetTransientFor(this);
        shortcutsWindow.SetIconName(Aura.Active.AppInfo.ID);
        shortcutsWindow.Present();
    }

    private void Quit(Gio.SimpleAction sender, EventArgs e) {
        if (!OnCloseRequested(this, EventArgs.Empty)) {
            _application.Quit();
        }
    }

    private void About(Gio.SimpleAction sender, EventArgs e) {
        var aboutDialog = new AboutDialog(window: this);
        aboutDialog.Present();
    }

    public void NewInstance(Gio.SimpleAction sender, EventArgs e) {
        var dialog = new NewInstanceDialog(_controller.CreateNewInstanceController()) {
            TransientFor = this
        };
        dialog.Present();
    }

    private void CreateAction(string name,
        GObject.SignalHandler<Gio.SimpleAction, Gio.SimpleAction.ActivateSignalArgs> action,
        string[] accel = null) {
        var act = Gio.SimpleAction.New(name, null);
        act.OnActivate += action;
        AddAction(act);
        if (accel != null) {
            _application.SetAccelsForAction($"win.{name}", accel);
        }
    }
}