using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day14;

internal static partial class Program {
  private const string Title = "\n## Day 14: Space Stoichiometry ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/14";
  private const long ExpectedPartOne = 443537;
  private const long ExpectedPartTwo = 0;
  private const bool Verbose = false;

  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      Reaction[] reactions = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(reactions);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(reactions);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static Reaction[] GetData(string filePath) {
    if (string.IsNullOrWhiteSpace(filePath)) {
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    List<Reaction> reactions = [];
    foreach (var line in data) {
      List<Chemical> chemicals = [];
      var matches = MyRegex.InputRegex().Matches(line);
      foreach (Match m in matches) {
        var items = m.Value.Split(' ', StringSplitOptions.None);
        chemicals.Add(new Chemical(name: items[1], amount: int.Parse(items[0])));
      }
      reactions.Add(new Reaction(chemicals.ToArray()));
    }

    return reactions.ToArray();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }

  internal static partial class MyRegex {
    [GeneratedRegex(@"(\d+\s\w+)", RegexOptions.Singleline)]
    public static partial Regex InputRegex();
  }

  internal static void ConsoleWriteLine(string message){
    if (Verbose){
      Console.WriteLine(message);
    }
  }

}
