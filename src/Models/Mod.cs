namespace Pickaxe.Models;

public enum ModSource {
    Local,
    CurseForge,
    Modrinth
}

public interface IMod {
    public ModSource Source { get; set; }
}

public class InstalledMod : IMod {
    public bool IsEnabled { get; set; } = true;
    public ModSource Source { get; set; }
}