using System.Diagnostics;
using AocHelper;

namespace Day01;

internal static class Program
{
  private const long ExpectedPartOne = 3226822;
  private const long ExpectedPartTwo = 4837367;

  public static int Main(string[] args)
  {
    PrintTitle();
    var input = GetData(args).ToIntArray();
    var stopwatch = Stopwatch.StartNew();

    var resultPartOne = PartOne(input);
    PrintResult("1", resultPartOne.ToString(), stopwatch);

    var resultPartTwo = PartTwo(input);
    PrintResult("2", resultPartTwo.ToString(), stopwatch);

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne(int[] modules)
  {
    long tally = 0;

    for (int i = 0; i < modules.Length; i++)
      tally += modules[i] / 3 - 2;

    return tally;
  }

  private static long PartTwo(int[] modules)
  {
    long tally = 0;

    for (int i = 0; i < modules.Length; i++)
    {
      var m = modules[i];

      while (true)
      {
        var fuel = m / 3 - 2;
        if (fuel <= 0)
          break;

        tally += fuel;
        m = fuel;
      }
    }

    return tally;
  }

  private static string[] GetData(string[] args)
  {
    var filename = "sample.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    return data;
  }


  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 1: The Tyranny of the Rocket Equation ##");
    Console.WriteLine("https://adventofcode.com/2019/day/1");
    Console.WriteLine();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo}");
    Console.WriteLine($"Result: {result}");
    Console.WriteLine($"Time elapsed (ms): {sw.Elapsed.TotalMilliseconds}");
    Console.WriteLine();
    sw.Restart();
  }
}
