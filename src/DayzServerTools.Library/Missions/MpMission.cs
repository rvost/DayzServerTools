using DayzServerTools.Library.Xml;

namespace DayzServerTools.Library.Missions;

public class MpMission
{
    public string MissionFolder { get; }

    public string Name { get; }
    public string MapName { get; }

    public EconomyCore EconomyCore { get; }
    public LimitsDefinitions LimitsDefinitions { get; }
    public UserDefinitions UserDefinitions { get; }
    public RandomPresets RandomPresets { get; }

    public MpMission(string path, EconomyCore economyCore, LimitsDefinitions limitsDefinitions,
        UserDefinitions userDefinitions, RandomPresets randomPresets)
    {
        MissionFolder = path.TrimEnd(Path.DirectorySeparatorChar);

        var split = Path.GetFileName(MissionFolder).Split(".");

        Name = split[0];
        MapName = split[1];

        EconomyCore = economyCore;
        LimitsDefinitions = limitsDefinitions;
        UserDefinitions = userDefinitions;
        RandomPresets = randomPresets;
    }

    public static async Task<MpMission> Open(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Mission folder\"{path}\" does not exists", nameof(path));
        }

        var openEconomy = Task.Run(() => EconomyCore.ReadFromFile(Path.Combine(path, MissionFiles.EconomyCore)));
        var openLimits = Task.Run(() => LimitsDefinitions.ReadFromFile(Path.Combine(path, MissionFiles.LimitsDefinitions)));
        var openUserDef = Task.Run(() => UserDefinitions.ReadFromFile(Path.Combine(path, MissionFiles.UserDefinitions)));
        var openPresets = Task.Run(() => RandomPresets.ReadFromFile(Path.Combine(path, MissionFiles.RandomPresets)));

        await Task.WhenAll(openEconomy, openLimits, openUserDef, openPresets);

        return new MpMission(path, openEconomy.Result, openLimits.Result, openUserDef.Result, openPresets.Result);
    }
}
