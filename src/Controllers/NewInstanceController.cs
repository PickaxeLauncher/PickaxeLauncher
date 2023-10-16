using System.Threading.Tasks;
using Pickaxe.Models;

namespace Pickaxe.Controllers;


public class NewInstanceController {
    public Instance Instance { get; set; } = new();
    private InstanceLoader _loader;

    public NewInstanceController(InstanceLoader loader) {
        _loader = loader;
    }

    public async Task CreateInstance() {
        _loader.Add(Instance);
        await Instance.Save();
    }
}