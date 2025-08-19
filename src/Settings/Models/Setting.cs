using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Settings.Models;

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