using System.Diagnostics;

namespace Day03;

internal static partial class Program
{
  public static int Main(string[] args)
  {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      (char dir, int val)[][] input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static (char dir, int val)[][] GetData(string filePath) {
    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    Debug.Assert(data.Length == 2, $"'data' must contain 2 sets of values. Value:'{data.Length}'");

    var wires = new (char dir, int val)[2][];
    wires[0] = [.. data[0].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => (d[0], int.Parse(d[1..])))];
    wires[1] = [.. data[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => (d[0], int.Parse(d[1..])))];

    return wires;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}

