using System.Diagnostics;

namespace Day16;

internal static partial class Program {
  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    string resultPartOne = "";
    string resultPartTwo = "";

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      List<int[]> input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input.First());
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input.Last());
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static List<int[]> GetData(string filePath) {
    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
    List<int[]> signals = [];
    foreach(var line in data){
      var chars = line.ToCharArray();
      var signal = new int[chars.Length];
      for (var i = 0; i < chars.Length; i++){
        signal[i] = chars[i] - '0';
      }
      signals.Add(signal);
    }
    Debug.Assert(signals.Count == 2, $"Expected 2 sets of data, Found: {signals.Count}");
    return signals;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
