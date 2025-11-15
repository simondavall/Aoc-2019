using System.Diagnostics;
using AocHelper;

namespace Day01;

internal static class Program
{
  private const long ExpectedPartOne = 3226822;
  private const long ExpectedPartTwo = 4837367;

  public static int Main(string[] args)
  {
    long resultPartOne = -1;
    long resultPartTwo = -1;

    PrintTitle();
    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      int[] input = GetData(filePath).ToIntArray();
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

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

  private static string[] GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath)){
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    return data;
  }

  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 1: The Tyranny of the Rocket Equation ##");
    Console.WriteLine("https://adventofcode.com/2019/day/1");
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
