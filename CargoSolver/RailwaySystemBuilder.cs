using CargoSolver.Models;

namespace CargoSolver;

public class RailwaySystemBuilder
{
    public RailwaySystem BuildRailwaySystem(string filePath)
    {
        using var reader = new StreamReader(filePath);

        var (stationCount, trackCount) = ParseHeader(reader);
        var stations = ParseStations(reader, stationCount);
        var tracks = ParseTracks(reader, trackCount);
        var startingStation = stations[ParseStartingStationId(reader)];

        return new RailwaySystem(stations.Values.ToList(), tracks, startingStation);
    }

    private static string[] ParseLine(StreamReader reader)
    {
        return reader.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    private static (int StationCount, int TrackCount) ParseHeader(StreamReader reader)
    {
        var parts = ParseLine(reader);
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static Dictionary<int, Station> ParseStations(StreamReader reader, int count)
    {
        var stations = new Dictionary<int, Station>(count);
        for (int i = 0; i < count; i++)
        {
            var parts = ParseLine(reader);
            var station = new Station
            {
                Id = int.Parse(parts[0]),
                CUnload = int.Parse(parts[1]),
                CLoad = int.Parse(parts[2])
            };
            stations[station.Id] = station;
        }
        return stations;
    }

    private static List<Track> ParseTracks(StreamReader reader, int count)
    {
        var tracks = new List<Track>(count);
        for (int i = 0; i < count; i++)
        {
            var parts = ParseLine(reader);
            tracks.Add(new Track
            {
                SourceId = int.Parse(parts[0]),
                DestinationId = int.Parse(parts[1])
            });
        }
        return tracks;
    }

    private static int ParseStartingStationId(StreamReader reader)
    {
        return int.Parse(reader.ReadLine()!.Trim());
    }
}
