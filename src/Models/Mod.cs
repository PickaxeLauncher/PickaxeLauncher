namespace Pickaxe.Models;

public enum ModSource {
    Local,
    CurseForge,
    Modrinth
}

public interface IMod {
    public string Name { get; }
    public ModSource Source { get; }
}

public class InstalledMod : IMod {
    public bool IsEnabled { get; set; } = true;
    public string Name { get; set; }
    public ModSource Source { get; set; }
}