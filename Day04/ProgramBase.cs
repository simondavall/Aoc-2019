using System.Diagnostics;

namespace Day04;

internal static partial class Program {
  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    var stopwatch = Stopwatch.StartNew();
    Console.WriteLine();

    var resultPartOne = PartOne();
    PrintResult("1", resultPartOne.ToString(), stopwatch);

    var resultPartTwo = PartTwo();
    PrintResult("2", resultPartTwo.ToString(), stopwatch);

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
