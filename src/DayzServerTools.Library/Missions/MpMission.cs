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
}
