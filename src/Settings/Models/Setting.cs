using Avolutions.BAF.Core.Entity.Models;

namespace Avolutions.BAF.Core.Settings.Models;

public class Setting : EntityBase
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Group { get; set; } = null!;
    public override string GetName()
    {
        throw new NotImplementedException();
    }
}