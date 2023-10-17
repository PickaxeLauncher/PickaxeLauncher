using Nickvision.Aura.Taskbar;
using Nickvision.Aura.Update;
using Pickaxe.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pickaxe.Controllers;

public class MainWindowController : IDisposable {
    private bool _disposed;
    private TaskbarItem _taskbarItem;
    public AccountController AccountController { get; } = new();
    public InstanceLoader InstanceLoader { get; } = new();

    public TaskbarItem TaskbarItem {
        set {
            if (value == null) {
                return;
            }

            _taskbarItem = value;
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public PreferencesController CreatePreferencesViewController() => new();
    public NewInstanceController CreateNewInstanceController() => new(InstanceLoader);

    public async Task StartupAsync() {
        await AccountController.StartupAsync();
        await InstanceLoader.StartupAsync();
    }

    ~MainWindowController() => Dispose(false);

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        _taskbarItem?.Dispose();
        _disposed = true;
    }
}