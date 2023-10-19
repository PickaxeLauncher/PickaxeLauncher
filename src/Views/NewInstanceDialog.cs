#pragma warning disable CA1416
using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;
using Nickvision.GirExt;
using Pickaxe.Controllers;
using Pickaxe.Helpers;
using Pickaxe.Models;

namespace Pickaxe.Views;


public class NewInstanceDialog : Adw.Window {
    private const string STAGE_1 = "stage1";
    private const string STAGE_2 = "stage2";

    readonly NewInstanceController _controller;

    [Gtk.Connect] private readonly Gtk.CheckButton _vanillaCheck = null;
    [Gtk.Connect] private readonly Gtk.CheckButton _forgeCheck = null;
    [Gtk.Connect] private readonly Gtk.CheckButton _fabricCheck = null;
    [Gtk.Connect] private readonly Adw.EntryRow _nameEntry = null;
    [Gtk.Connect] private readonly Gtk.Button _changeIconButton = null;
    [Gtk.Connect] private readonly Gtk.Picture _icon = null;
    [Gtk.Connect] private readonly Gtk.Button _backButton = null;
    [Gtk.Connect] private readonly Gtk.Button _nextButton = null;
    [Gtk.Connect] private readonly Adw.NavigationView _navView = null;
    [Gtk.Connect] private readonly Adw.PreferencesPage _versionsPage = null;

    public NewInstanceDialog(NewInstanceController controller) : this(
        new Gtk.Builder("new_instance_dialog.ui"), controller) {
    }

    private NewInstanceDialog(Gtk.Builder builder, NewInstanceController controller) : base(
        builder.GetPointer("_root"), false) {
        builder.Connect(this);
        _controller = controller;
        Modal = true;
        _icon.SetResource("/dev/bedsteler20/Pickaxe/minecraft.svg");
        _changeIconButton.OnClicked += ChangeIcon;
        _backButton.OnClicked += OnBackButton;
        _nextButton.OnClicked += NextStep;
    }

    private void OnBackButton(Gtk.Button sender, EventArgs args) {
        var success = _navView.Pop();
        if (!success) Close();
    }

    private void NextStep(Gtk.Button sender, EventArgs args) {
        switch (_navView.VisiblePage.Tag) {
            case STAGE_1:
                // TODO: Check if name is duplicate
                if (_nameEntry.GetText() == "") {
                    ShowErrorMessage("The instance must have a name");
                    return;
                }
                _navView.PushByTag(STAGE_2);
                BuildVersions();
                break;
        }
    }

    private void ShowErrorMessage(string msg) {
        var dialog = new Adw.MessageDialog() {
            Body = msg,
            TransientFor = this
        };
        dialog.AddResponse("ok", "Ok");
        dialog.Present();
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
            _controller.Instance.Icon = file.GetPath();
            _icon.SetPixbuf(pixbuf);
        });
    }

    public void BuildVersions() {
        var loader = new CmlLib.Core.VersionLoader.MojangVersionLoader();

        Gtk.CheckButton btnGroup = new();
        Adw.ExpanderRow releases = new() {
            Title = "Releases",
            Expanded = true
        };
        Adw.PreferencesGroup releasesGroup = new();
        releasesGroup.Add(releases);

        foreach (var version in loader.GetVersionMetadatas()) {
            Gtk.CheckButton button = new() {
                Group = btnGroup
            };

            Adw.ActionRow row = new() {
                Title = version.Name,
                ActivatableWidget = button
            };
            row.AddSuffix(button);
            switch (version.MType) {
                case MVersionType.Release:
                    releases.AddRow(row);
                    break;
            }
        }

        _versionsPage.Add(releasesGroup);
    }

    public Gtk.Widget BuildVersionList(MVersionCollection collect, Gtk.CheckButton group, string title, bool expanded) {
        Adw.ExpanderRow expanderRow = new() {
            Title = title,
            Expanded = expanded
        };
        Adw.PreferencesGroup preferencesGroup = new();
        preferencesGroup.Add(expanderRow);
    }

}
#pragma warning restore CA1416
