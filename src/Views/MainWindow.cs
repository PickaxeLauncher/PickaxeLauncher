using Nickvision.Aura.Taskbar;
using Pickaxe.Helpers;
using Pickaxe.Controllers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static Nickvision.GirExt.GtkExt;
using static Pickaxe.Helpers.Gettext;
using Nickvision.Aura;
using XboxAuthNet.Game.Msal;
using CmlLib.Core.Auth.Microsoft;

namespace Pickaxe.Views;

/// <summary>
/// The MainWindow for the application
/// </summary>
public partial class MainWindow : Adw.ApplicationWindow {
    private readonly MainWindowController _controller;
    private readonly Adw.Application _application;

    [Gtk.Connect] private readonly Adw.WindowTitle _title;

    public MainWindow(MainWindowController controller, Adw.Application application) : this(Builder.FromFile("window.ui"), controller, application) { }

    private MainWindow(Gtk.Builder builder, MainWindowController controller, Adw.Application application) : base(builder.GetPointer("_root"), false) {
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

        CreateAction("preferences", Preferences, new string[] { "<Ctrl>comma" });
        CreateAction("keyboardShortcuts", KeyboardShortcuts, new string[] { "<Ctrl>question" });
        CreateAction("quit", Quit, new string[] { "<Ctrl>q" });
        CreateAction("about", About, new string[] { "F1" });

    }



    public async Task StartAsync() {
        _application.AddWindow(this);
        Present();
        _controller.TaskbarItem = await TaskbarItem.ConnectLinuxAsync($"{Aura.Active.AppInfo.ID}.desktop");
        await _controller.StartupAsync();
        if (_controller.AccountController.Accounts.Count == 0) {
            await _controller.AccountController.AddAccount();
        }
    }

    private bool OnCloseRequested(Gtk.Window sender, EventArgs e) {
        _controller.Dispose();
        return false;
    }


    private void Preferences(Gio.SimpleAction sender, EventArgs e) {
        var preferencesDialog = new PreferencesDialog(_controller.CreatePreferencesViewController(), _application, this);
        preferencesDialog.Present();
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

    private void CreateAction(string name, GObject.SignalHandler<Gio.SimpleAction, Gio.SimpleAction.ActivateSignalArgs> action, string[] accel = null) {
        var act = Gio.SimpleAction.New(name, null);
        act.OnActivate += action;
        AddAction(act);
        if (accel != null) {
            _application.SetAccelsForAction($"win.{name}", accel);
        }
    }
}
