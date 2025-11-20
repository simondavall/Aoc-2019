using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day07;

internal static class Program {
  private const long ExpectedPartOne = 880726;
  private const long ExpectedPartTwo = 4931744;

  public static int Main(string[] args) {
    Console.WriteLine("\n## Day 7: Amplification Circuit ##");
    Console.WriteLine("https://adventofcode.com/2019/day/7");

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
    long maxSignal = 0;

    foreach (var phase in GetPhasingSequences(0, 4)) {
      long lastOutput = 0;
      bool phaseInput = true;
      for (var i = 0; i < 5; i++) {
        var amp = new IntcodeComputer(program);
        while (!amp.IsHalted) {
          if (amp.IsAwaitingInput) {
            if (phaseInput)
              amp.SetInput(phase[i]);
            else
              amp.SetInput(lastOutput);
            phaseInput = !phaseInput;
          }
          amp.Execute();
        }
        lastOutput = amp.GetLastOutput;
      }
      maxSignal = Math.Max(maxSignal, lastOutput);
    }

    return maxSignal;
  }

  private static long PartTwo(long[] program) {
    long maxSignal = 0;

    foreach (var phase in GetPhasingSequences(5, 9)) {
      var phaseInput = Helper.CreateArray(5, true);

      var amps = new IntcodeComputer[5];
      for (var i = 0; i < 5; i++)
        amps[i] = new IntcodeComputer(program);

      var cur = 0;
      while (!amps[4].IsHalted) {
        if (amps[cur].IsAwaitingInput) {
          if (phaseInput[cur]) {
            amps[cur].SetInput(phase[cur]);
            phaseInput[cur] = false;
          } else {
            var prevAmpOutput = amps[(cur + 4) % 5].GetLastOutput;
            amps[cur].SetInput(prevAmpOutput);
          }
        }
        amps[cur].Execute();

        cur = (cur + 1) % 5;
      }

      maxSignal = Math.Max(maxSignal, amps[4].GetLastOutput);
    }

    return maxSignal;
  }

  private static long[] GetData(string filePath) {
    using var streamReader = new StreamReader(filePath);
    var data = streamReader
      .ReadToEnd()
      .Split(',', StringSplitOptions.RemoveEmptyEntries)
      .ToLongArray();

    return data;
  }

  private static List<int[]> GetPhasingSequences(int min, int max) {
    var combos = new List<int[]>();
    for (int i = min; i < max + 1; i++)
      for (int j = min; j < max + 1; j++)
        if (i != j)
          for (int k = min; k < max + 1; k++)
            if (k != i && k != j)
              for (int l = min; l < max + 1; l++)
                if (l != i && l != j && l != k)
                  for (int m = min; m < max + 1; m++)
                    if (m != i && m != j && m != k && m != l) {
                      combos.Add([i, j, k, l, m]);
                    }
    return combos;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
