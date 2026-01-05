using NU1390.Abstracts;

namespace NU1390.Additional;

public class Impl1 : MyClassBase
{
    public override string Route { get; set; } = "impl1";

    public string Field1 { get; set; } = string.Empty;
}