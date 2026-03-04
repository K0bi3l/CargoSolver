namespace CargoSolver.Models;

public class Station
{
    public int Id { get; init; }
    public int CUnload { get; init; }
    public int CLoad { get; init; }
    public HashSet<int> PossibleLoads { get; } = new HashSet<int>();
}
