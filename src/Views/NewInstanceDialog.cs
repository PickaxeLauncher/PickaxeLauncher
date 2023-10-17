using System;
using System.Threading.Tasks;
using Nickvision.GirExt;
using Pickaxe.Controllers;
using Pickaxe.Models;

namespace Pickaxe.Views;

public class NewInstanceDialog : Adw.Window {
    readonly NewInstanceController _controller;

    [Gtk.Connect] private readonly Gtk.CheckButton _vanilla_check = null;
    [Gtk.Connect] private readonly Gtk.CheckButton _forge_check = null;
    [Gtk.Connect] private readonly Gtk.CheckButton _fabric_check = null;
    [Gtk.Connect] private readonly Adw.EntryRow _name_entry = null;
    [Gtk.Connect] private readonly Gtk.Button _change_icon_button = null;

    public NewInstanceDialog(NewInstanceController controller) : this(
        new Gtk.Builder("new_instance_dialog.ui"), controller) {
    }

    private NewInstanceDialog(Gtk.Builder builder, NewInstanceController controller) : base(
        builder.GetPointer("_root"), false) {
        _controller = controller;
        builder.Connect(this);
        Modal = true;
        _change_icon_button.OnClicked += ChangeIcon;
    }

    private void ChangeIcon(Gtk.Button sender, EventArgs args) => Task.Run(async () => {
        var picker = Gtk.FileDialog.New();
        var file = picker.OpenAsync(this);
        if (file is null) return;
        await _controller.ChangeIcon(file);
    });

}