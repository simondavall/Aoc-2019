using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day13;

internal static class Program {
  private const long ExpectedPartOne = 273;
  private const long ExpectedPartTwo = 13140;

  public static int Main(string[] args) {
    Console.WriteLine("\n## Day 13: Care Package ##");
    Console.WriteLine("https://adventofcode.com/2019/day/13");

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

  private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);
    while (!computer.IsHalted) {
      computer.Execute();
   }
    long tally = 0;
    var output = computer.FullOutput;
    for (var i = 2; i < output.Length; i += 3) {
      if (output[i] == 2) {
        tally++;
      }
    }

    return tally;
  }

  private static long PartTwo(long[] program) {
    var computer = new IntcodeComputer(program);
    computer.SetMemory(0, 2);

    var idx = 0;
    long currentScore = 0;
    long ball = 0, paddle = 0;

    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        var move = ball.CompareTo(paddle);
        computer.SetInput(move);
      }
      computer.Execute();
       var output = computer.FullOutput;
      for (var i = idx; i < output.Length; i += 3){
        if (output[i] == -1 && output[i + 1] == 0){
          currentScore = output[i + 2];
        }
        if (output[i + 2] == 4){
          ball = output[i];
        } else if (output[i + 2] == 3){
          paddle = output[i];
        }
      }
      idx = output.Length;
    }

    return currentScore;
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
