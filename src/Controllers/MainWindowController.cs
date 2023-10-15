using Nickvision.Aura.Taskbar;
using Nickvision.Aura.Update;
using Pickaxe.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pickaxe.Controllers;

public class MainWindowController : IDisposable {
    public AccountController AccountController { get; } = new();
    private bool _disposed;
    private TaskbarItem _taskbarItem;


    public TaskbarItem TaskbarItem {
        set {
            if (value == null) {
                return;
            }
            _taskbarItem = value;
        }
    }

    public PreferencesController CreatePreferencesViewController() => new();

    public async Task StartupAsync() {
        await AccountController.StartupAsync();
    }

    ~MainWindowController() => Dispose(false);

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }
        _taskbarItem?.Dispose();
        _disposed = true;
    }
}
