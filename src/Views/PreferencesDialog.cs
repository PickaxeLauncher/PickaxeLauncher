using Pickaxe.Helpers;
using Pickaxe.Controllers;
using Pickaxe.Models;
using System;
using Nickvision.Aura;

namespace Pickaxe.Views;

/// <summary>
/// The PreferencesDialog for the application
/// </summary>
public partial class PreferencesDialog : Adw.PreferencesWindow {
    private readonly PreferencesController _controller;
    private readonly Adw.Application _application;

    public PreferencesDialog(PreferencesController controller, Adw.Application application, Gtk.Window parent) : this(Builder.FromFile("preferences_dialog.ui"), controller, application, parent) { }

    private PreferencesDialog(Gtk.Builder builder, PreferencesController controller, Adw.Application application, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        _controller = controller;
        _application = application;
        SetTransientFor(parent);
        SetIconName(Aura.Active.AppInfo.ID);
        builder.Connect(this);
        OnHide += Hide;
    }

    private void Hide(Gtk.Widget sender, EventArgs e) {
        _controller.SaveConfiguration();
        Destroy();
    }
}
