using System;
using System.IO;
using System.Threading.Tasks;
using Nickvision.GirExt;
using Pickaxe.Controllers;
using Pickaxe.Helpers;
using Pickaxe.Models;

namespace Pickaxe.Views;

public class NewInstanceDialog : Adw.Window {
    readonly NewInstanceController _controller;

    [Gtk.Connect] private readonly Gtk.CheckButton _vanilla_check = null;
    [Gtk.Connect] private readonly Gtk.CheckButton _forge_check = null;
    [Gtk.Connect] private readonly Gtk.CheckButton _fabric_check = null;
    [Gtk.Connect] private readonly Adw.EntryRow _name_entry = null;
    [Gtk.Connect] private readonly Gtk.Button _change_icon_button;
    [Gtk.Connect] private readonly Gtk.Picture _icon = null;

    public NewInstanceDialog(NewInstanceController controller) : this(
        new Gtk.Builder("new_instance_dialog.ui"), controller) {
    }

    private NewInstanceDialog(Gtk.Builder builder, NewInstanceController controller) : base(
        builder.GetPointer("_root"), false) {
        builder.Connect(this);
        _controller = controller;
        Modal = true;
        _change_icon_button.OnClicked += ChangeIcon;
    }

    private void ChangeIcon(Gtk.Button sender, EventArgs args) {
        var _icon_picker = Gtk.FileDialog.New();
        var filter = Gtk.FileFilter.New();
        filter.AddPixbufFormats();
        _icon_picker.SetDefaultFilter(filter);
        const int ICON_SIZE = 100;
        var task = _icon_picker.OpenAsync(this);
        task.GetAwaiter().OnCompleted(() => {
            var file = task.Result;
            if (file == null) return;
            GdkPixbuf.Pixbuf pixbuf = GdkPixbuf.Pixbuf.NewFromFile(file.GetPath());

            if (pixbuf.Width != pixbuf.Height) {
                int smallestSide = Math.Min(pixbuf.Width, pixbuf.Height);
                pixbuf = pixbuf.NewSubpixbuf(0, 0, smallestSide, smallestSide);
            }
            if (pixbuf.Width != ICON_SIZE && pixbuf.Height != ICON_SIZE) {
                pixbuf = pixbuf.ScaleSimple(ICON_SIZE, ICON_SIZE, GdkPixbuf.InterpType.Bilinear);
            }
            _icon.SetPixbuf(pixbuf);
        });
    }

}