using System.Diagnostics;

namespace Day03;

internal static class Program
{
  private const long ExpectedPartOne = 0;
  private const long ExpectedPartTwo = 0;

  private static readonly Dictionary<char, (int dr, int dc)> Directions = new() { { 'U', (-1, 0) }, { 'R', (0, 1) }, { 'D', (1, 0) }, { 'L', (0, -1) } };

  public static int Main(string[] args)
  {
    PrintTitle();
    var input = GetData(args);
    var stopwatch = Stopwatch.StartNew();

    var resultPartOne = PartOne(input);
    PrintResult("1", resultPartOne.ToString(), stopwatch);

    var resultPartTwo = PartTwo(input);
    PrintResult("2", resultPartTwo.ToString(), stopwatch);

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne((char, int)[][] wires)
  {
    long minDist = int.MaxValue;

    var visited = new HashSet<(int, int)>();

    // first wire path
    int r = 0, c = 0;
    foreach (var (dir, val) in wires[0])
    {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++)
      {
        r += dr;
        c += dc;
        visited.Add((r, c));
      }
    }

    // compare second wire path with first
    r = 0;
    c = 0;
    foreach (var (dir, val) in wires[1])
    {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++)
      {
        r += dr;
        c += dc;
        if (visited.Contains((r, c)))
        {
          var dist = Math.Abs(r) + Math.Abs(c);
          if (minDist > dist)
            minDist = dist;
        }
      }
    }

    return minDist;
  }

  private static long PartTwo((char, int)[][] wires)
  {
    long minDist = int.MaxValue;

    var visited = new Dictionary<(int, int), int>();

    // first wire path
    int r = 0, c = 0, steps = 0;
    foreach (var (dir, val) in wires[0])
    {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++)
      {
        steps++;
        r += dr;
        c += dc;
        if (!visited.ContainsKey((r,c)))
          visited.Add((r, c), steps);
      }
    }

    // compare second wire path with first
    r = 0;
    c = 0;
    steps = 0;
    foreach (var (dir, val) in wires[1])
    {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++)
      {
        steps++;
        r += dr;
        c += dc;
        if (visited.ContainsKey((r, c)))
        {
          var dist = steps + visited[(r,c)];
          if (minDist > dist)
            minDist = dist;
        }
      }
    }

    return minDist;
  }

  private static (char dir, int val)[][] GetData(string[] args)
  {
    var filename = "sample.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    Debug.Assert(data.Length == 2, $"'data' must contain 2 sets of values. Value:'{data.Length}'");

    var wires = new (char dir, int val)[2][];
    wires[0] = [.. data[0].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => (d[0], int.Parse(d[1..])))];
    wires[1] = [.. data[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => (d[0], int.Parse(d[1..])))];

    return wires;
  }


  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 3: Crossed Wires ##");
    Console.WriteLine("https://adventofcode.com/2019/day/3");
    Console.WriteLine();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo}\\");
    Console.WriteLine($"Result: {result}\\");
    Console.WriteLine($"Time elapsed (ms): {sw.Elapsed.TotalMilliseconds}");
    Console.WriteLine();
    sw.Restart();
  }
}
