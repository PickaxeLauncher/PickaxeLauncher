namespace Pickaxe.Models;

public interface IModLoader { }

public class FabricModLoader : IModLoader { }

public class ForgeModLoader : IModLoader { }

// Not really a mod loader ¯\_(ツ)_/¯
public class VanillaModLoader : IModLoader { }