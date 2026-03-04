using CargoSolver.Models;

namespace CargoSolver;

public class Solver
{
    public void FindPossibleLoads(RailwaySystem railwaySystem)
    {
        var isQueued = new HashSet<int>(railwaySystem.StationCount);

        Queue<Station> queue = new Queue<Station>();
        queue.Enqueue(railwaySystem.StartingStation);
        isQueued.Add(railwaySystem.StartingStation.Id);

        while (queue.Count > 0)
        {
            var currentStation = queue.Dequeue();
            isQueued.Remove(currentStation.Id);

            var departureLoad = new HashSet<int>(currentStation.PossibleLoads);
            departureLoad.Remove(currentStation.CUnload);
            departureLoad.Add(currentStation.CLoad);

            foreach (var neighbor in railwaySystem.GetNeighbors(currentStation))
            {
                var addLoad = new HashSet<int>(departureLoad);
                addLoad.ExceptWith(neighbor.PossibleLoads);

                if (addLoad.Count != 0)
                {
                    neighbor.PossibleLoads.UnionWith(addLoad);
                    if (isQueued.Add(neighbor.Id))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }
}
