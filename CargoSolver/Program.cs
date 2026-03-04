namespace CargoSolver;

public class Program
{
    static void Main(string[] args)
    {
        var builder = new RailwaySystemBuilder();
        var solver = new Solver();
        while (true)
        {
            string filePath;
            while (true)
            {
                Console.WriteLine("Please enter the path to the input file:");
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                    continue;
                }
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting the program.");
                    return;
                }
                if (!File.Exists(input))
                {
                    Console.WriteLine("File does not exist. Please try again.");
                    continue;
                }
                filePath = input;
                break;
            }
            Console.WriteLine($"Processing file: {filePath}");
            Console.WriteLine("Possible loads: ");
            var railwaySystem = builder.BuildRailwaySystem(filePath);
            solver.FindPossibleLoads(railwaySystem);
            railwaySystem.PrintPossibleLoads();
        }
    }
}
