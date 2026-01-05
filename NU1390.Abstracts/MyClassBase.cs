namespace NU1390.Abstracts;

public class MyClassBase
{
    public virtual string Route { get; set; } = "#";
}

public class Dto<T> where T : MyClassBase
{
    public long? Index { get; set; }
    public T Payload { get; set; } = null!;
}