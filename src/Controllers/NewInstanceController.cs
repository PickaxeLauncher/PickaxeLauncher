using System.Threading.Tasks;
using Pickaxe.Models;

namespace Pickaxe.Controllers;

public class NewInstanceController {
    private readonly InstanceLoader _loader;

    public NewInstanceController(InstanceLoader loader) {
        _loader = loader;
    }

    public Instance Instance { get; set; } = new();

    public async Task CreateInstance() {
        _loader.Add(Instance);
        await Instance.Save();
    }
}