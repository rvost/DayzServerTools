using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Library.Test;

public class EconomyCoreTests
{
    [Fact]
    public void CanDeserialize()
    {
        using var input = File.OpenRead("Resources\\cfgeconomycore.xml");

        var obj = EconomyCore.ReadFromStream(input);

        Assert.NotNull(obj);

        Assert.NotEmpty(obj.CeFolders);
        Assert.Equal(7, obj.Classes.Count);

        Assert.NotEmpty(obj.Defaults);
        Assert.Equal(19, obj.Defaults.Count);

        var rootclassA = obj.Classes.Where(x => x.Name == "DefaultWeapon").FirstOrDefault();
        Assert.NotNull(rootclassA);
        Assert.True(rootclassA.ReportMemoryLOD);

        var rootclassB = obj.Classes.Where(x => x.Name == "HouseNoDestruct").FirstOrDefault();
        Assert.NotNull(rootclassB);
        Assert.False(rootclassB.ReportMemoryLOD);
    }

    [Fact]
    public void TrySerialize()
    {
        var obj = new EconomyCore();
        obj.Classes.Add(new() { Name = "DefaultWeapon" });
        obj.Classes.Add(new() { Name = "DefaultMagazine" });
        obj.Classes.Add(new() { Name = "HouseNoDestruct", ReportMemoryLOD = false });

        obj.Defaults.Add(new() { Name = "dyn_radius", Value = "30" });
        obj.Defaults.Add(new() { Name = "log_ce_dynamicevent", Value = "false" });

        var ceFolder = new CeFolder() { Folder = "db" };
        ceFolder.Files.Add(new() { Name = "types_dzn.xml", Type = CeFileType.Types });
        ceFolder.Files.Add(new() { Name = "spawnables_dzn.xml", Type = CeFileType.SpawnableTypes });
        obj.CeFolders.Add(ceFolder);

        using var output = File.Create("serialized.xml");
        obj.WriteToStream(output);
    }

    [Fact]
    public void CanValidate()
    {
        var ceFolder = new CeFolder() { Folder = "db" };
        ceFolder.Files.Add(new() { Name = "types_dzn.xml", Type = CeFileType.Types });
        ceFolder.Files.Add(new() { Name = "spawnables_dzn.xml", Type = CeFileType.SpawnableTypes });
        
        var obj = new EconomyCore();
        obj.CeFolders.Add(ceFolder);
        obj.CeFolders.Add(new() { Folder= "foo" });

        var validator = new EconomyCoreValidator(@"Resources\mission.test");
        var result = validator.Validate(obj);
        
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count);
    }
}
