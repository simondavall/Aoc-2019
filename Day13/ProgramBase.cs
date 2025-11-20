using System.Diagnostics;
using AocHelper;

namespace Day13;

internal static partial class Program {
  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      long[] program = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(program);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(program);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long[] GetData(string filePath) {
    if (string.IsNullOrWhiteSpace(filePath)) {
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader
      .ReadToEnd()
      .Split(',', StringSplitOptions.RemoveEmptyEntries)
      .ToLongArray();

    return data;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}

