using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day07;

internal static class Program
{
  private const long ExpectedPartOne = 0;
  private const long ExpectedPartTwo = 0;

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

  private static long PartOne(long[] program)
  {
    long maxSignal = 0;

    foreach (var seq in GetPhasingSequences(0, 4))
    {
      var phasingSequence = new Queue<int>(seq);
      // Console.WriteLine($"{seq[0]},{seq[1]},{seq[2]},{seq[3]},{seq[4]}");
      var outputSignal = IntcodeComputer.Acs(phasingSequence, program);
      maxSignal = Math.Max(maxSignal, outputSignal);
    }

    return maxSignal;
  }

  private static long PartTwo(long[] program)
  {
    long maxSignal = 0;

    // foreach (var seq in GetPhasingSequences(5, 9))
    // {
    //   var phasingSequence = new Queue<int>(seq);
    //   //Console.WriteLine($"{seq[0]},{seq[1]},{seq[2]},{seq[3]},{seq[4]}");
    //   var outputSignal = IntcodeComputer.Acs(phasingSequence, program);
    //   maxSignal = Math.Max(maxSignal, outputSignal);
    // }
    //
    return maxSignal;
  }

  private static long[] GetData(string[] args)
  {
    var filename = "Day07/sample.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    var data = streamReader.ReadToEnd().Split(',', StringSplitOptions.RemoveEmptyEntries).ToLongArray();
    return data;
  }

  private static List<int[]> GetPhasingSequences(int min, int max)
  {
    var combos = new List<int[]>();
    for (int i = min; i < max + 1; i++)
      for (int j = min; j < max + 1; j++)
        if (i != j)
          for (int k = min; k < max + 1; k++)
            if (k != i && k != j)
              for (int l = min; l < max + 1; l++)
                if (l != i && l != j && l != k)
                  for (int m = min; m < max + 1; m++)
                    if (m != i && m != j && m != k && m != l)
                    {
                      combos.Add([i, j, k, l, m]);
                    }
    return combos;
  }

  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 7: Amplification Circuit ##");
    Console.WriteLine("https://adventofcode.com/2019/day/7");
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
