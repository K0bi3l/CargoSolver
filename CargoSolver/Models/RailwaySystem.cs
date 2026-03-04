namespace CargoSolver.Models;

public class RailwaySystem
{
    public int StationCount => stations.Count;

    public Station GetStationById(int id) => stations[id];

    public IEnumerable<int> GetAllStationsIds() => stations.Keys;

    private readonly Dictionary<int, Station> stations;
    private readonly Dictionary<int, List<Station>> adjacencyList;

    public Station StartingStation { get; }

    public RailwaySystem(IReadOnlyList<Station> stationList, IReadOnlyList<Track> trackList, Station startingStation)
    {
        stations = new Dictionary<int, Station>(stationList.Count);
        foreach (var station in stationList)
        {
            stations[station.Id] = station;
        }

        adjacencyList = [];
        foreach (var track in trackList)
        {
            if (!adjacencyList.TryGetValue(track.SourceId, out var neighbors))
            {
                neighbors = [];
                adjacencyList[track.SourceId] = neighbors;
            }
            neighbors.Add(stations[track.DestinationId]);
        }

        StartingStation = startingStation;
    }

    public IEnumerable<Station> GetNeighbors(Station node)
    {
        return adjacencyList.TryGetValue(node.Id, out var neighbors) ? neighbors : Enumerable.Empty<Station>();
    }

    public void PrintPossibleLoads()
    {
        var sorted = stations.Values.OrderBy(s => s.Id);

        foreach (var station in sorted)
        {
            Console.WriteLine($"Station {station.Id}: Possible Loads = {string.Join(", ", station.PossibleLoads)}");
        }
    }
}
