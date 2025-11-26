using AocHelper;
using Spacecraft;

namespace Day07;

internal static partial class Program {
  private const string Title = "\n## Day 7: Amplification Circuit ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/7";

  private const long ExpectedPartOne = 880726;
  private const long ExpectedPartTwo = 4931744;

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
        lastOutput = amp.GetOutput()[^1];
      }
      maxSignal = Math.Max(maxSignal, lastOutput);
    }

    return maxSignal;
  }

  private static long PartTwo(long[] program) {
    long maxSignal = 0;

    foreach (var phase in GetPhasingSequences(5, 9)) {
      bool[] phaseInput = Helper.CreateArray(5, true);

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
            var prevAmpOutput = amps[(cur + 4) % 5].GetOutput();
            if (prevAmpOutput.Length > 0)
              amps[cur].SetInput(prevAmpOutput[^1]);
            else
              amps[cur].SetInput(0);

          }
        }
        amps[cur].Execute();

        cur = (cur + 1) % 5;
      }

      maxSignal = Math.Max(maxSignal, amps[4].GetOutput()[^1]);
    }

    return maxSignal;
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
}
