#pragma warning disable CA1416
using System;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using Pickaxe.Controllers;
using Pickaxe.Helpers.Extension;

namespace Pickaxe.Views;

public class NewInstanceDialog : Adw.Window {
    private int _stage = 0;
    [Gtk.Connect] private readonly Gtk.Button _backButton = null;
    [Gtk.Connect] private readonly Gtk.Button _changeIconButton = null;
    [Gtk.Connect] private readonly Gtk.Picture _icon = null;
    [Gtk.Connect] private readonly Adw.EntryRow _nameEntry = null;
    [Gtk.Connect] private readonly Adw.NavigationView _navView = null;
    [Gtk.Connect] private readonly Gtk.Button _nextButton = null;
    [Gtk.Connect] private Gtk.Stack _version_stack = null;
    [Gtk.Connect] private Adw.ExpanderRow _release_row = null;
    [Gtk.Connect] private Adw.ExpanderRow _snapshot_row = null;
    [Gtk.Connect] private Gtk.Stack _installStack = null;
    [Gtk.Connect] private Gtk.ProgressBar _progressBar = null;
    [Gtk.Connect] private Adw.StatusPage _downloadPage = null;
    [Gtk.Connect] private Gtk.Button _completeButton = null;


    readonly NewInstanceController _controller;

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
        _controller.OnProgressChanged += (s, e) => _progressBar.Fraction = e;
        _controller.OnStatusChanged += (s, e) => _downloadPage.Title = e;
        _controller.OnDoneInstalling += (sender, args) => {
            _installStack.SetVisibleChildName("page_installed");
            _completeButton.OnClicked += (s, e) => Close();
        };
    }

    private void OnBackButton(Gtk.Button sender, EventArgs args) {
        _stage--;
        var success = _navView.Pop();
        if (!success) Close();
    }

    private void NextStep(Gtk.Button sender, EventArgs args) {
        switch (_stage) {
            case 0:
                LoadVersionsPage();
                break;
            case 1:
                LoadInstallPage();
                break;
        }
    }


    private void LoadVersionsPage() => Task.Run(async () => {
        _controller.SetName(_nameEntry.GetText());
        if (_controller.HasName() == false) {
            ShowErrorMessage("The instance must have a name");
            return;
        }
        _navView.PushByTag("version_page");
        _stage++;
        var group = new Gtk.CheckButton();
        var versions = await _controller.GetVersions();
        foreach (var item in versions) {
            var check = new Gtk.CheckButton() {
                Group = group
            };
            check.OnActivate += (sender, args) => {
                if (check.Active) {
                    _controller.SetVersion(item);
                }
            };
            var row = new Adw.ActionRow() {
                Title = item.Name,
                ActivatableWidget = check
            };
            row.AddSuffix(check);

            switch (item.MType) {
                case MVersionType.Release:
                    _release_row.AddRow(row);
                    break;
                case MVersionType.Snapshot:
                    _snapshot_row.AddRow(row);
                    break;
            }
        }
        _version_stack.SetVisibleChildName("_done_page");
    });


    private void LoadInstallPage() {
        if (_controller.HasVersion() == false) {
            ShowErrorMessage("You must select a version");
            return;
        }
        _navView.PushByTag("install_page");
        _controller.Install();
        _stage++;
        _backButton.Visible = false;
        _nextButton.Visible = false;
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
        var task = _icon_picker.OpenAsync(this);
        task.GetAwaiter().OnCompleted(() => {
            var file = task.Result;
            if (file == null) return;
            var pixbuf = GdkPixbuf.Pixbuf.NewFromFile(file.GetPath()).MakeSquare(100);
            _controller.SetIcon(pixbuf);
            _icon.SetPixbuf(pixbuf);
        });
    }

}


#pragma warning restore CA1416