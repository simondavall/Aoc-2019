using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day12;

internal static partial class Program {
  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCodde);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      Moon[] input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static Moon[] GetData(string filePath) {
    if (string.IsNullOrWhiteSpace(filePath)) {
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    List<Moon> moons = [];
    foreach (var line in data) {
      var m = MyRegex.InputRegex().Matches(line);
      if (m.Count != 3) {
        throw new ApplicationException($"Should find 3 input values. Fount:{m.Count}");
      }
      var x = int.Parse(m[0].Value);
      var y = int.Parse(m[1].Value);
      var z = int.Parse(m[2].Value);
      moons.Add(new Moon(x, y, z));
    }

    return moons.ToArray();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }

  internal static partial class MyRegex {
    [GeneratedRegex(@"(-?\d+)", RegexOptions.Singleline)]
    public static partial Regex InputRegex();
  }
}
