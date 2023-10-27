
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pickaxe.Services;

public abstract class AsyncService {
    public bool IsInitialized { get; private set; }
    public async Task Initialize() {
        if (IsInitialized) {
            return;
        }
        await OnInitialize();
        IsInitialized = true;
    }

    protected abstract Task OnInitialize();
}

public class ServiceNotFoundException : Exception {
    public ServiceNotFoundException(Type type) : base($"The service {type.FullName} was not fond") { }
}

public abstract class LazyAsyncService {
    public bool IsInitialized { get; private set; }
    public event EventHandler Initialized;

    public void Initialize() => Task.Run(async () => {
        if (IsInitialized) {
            return;
        }
        await OnInitialize();
        Initialized?.Invoke(this, null);
        IsInitialized = true;
    });

    protected abstract Task OnInitialize();
}

public interface INamedService {
    string ServiceName { get; }
}

public static class Injector {
    private static List<object> _instances = new();

    public static void InstallService(object instance) {
        _instances.Add(instance);
    }

    public static async Task InstallServiceAsync(AsyncService instance) {
        await instance.Initialize();
        _instances.Add(instance);
    }

    public static void UninstallService(object instance) {
        _instances.Remove(instance);
    }

    public static T Inject<T>() {
        foreach (var item in _instances) {
            if (item is T t) {
                return t;
            }
        }
        throw new ServiceNotFoundException(typeof(T));
    }

    public static T Inject<T>(string name) where T : INamedService {
        foreach (var item in _instances) {
            if (item is T t && t.ServiceName == name) {
                return t;
            }
        }
        throw new ServiceNotFoundException(typeof(T));
    }

    public static async Task Initialize() {
        foreach (var item in _instances) {
            if (item is AsyncService service) {
                await service.Initialize();
            }
            else if (item is LazyAsyncService lazyService) {
                lazyService.Initialize();
                lazyService.Initialized += (sender, service) => {
                    Console.WriteLine($"Lazy service {service.GetType().Name} initialized");
                };
            }
        }
    }

}