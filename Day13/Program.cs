using Spacecraft;

namespace Day13;

internal static partial class Program {
  private const string Title = "\n## Day 13: Care Package ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/13";

  private const long ExpectedPartOne = 273;
  private const long ExpectedPartTwo = 13140;

  private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);
    while (!computer.IsHalted) {
      computer.Execute();
    }
    long tally = 0;
    var output = computer.GetOutput();
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

    long currentScore = 0;
    long ball = 0, paddle = 0;

    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        var move = ball.CompareTo(paddle);
        computer.SetInput(move);
      }
      computer.Execute();
      var output = computer.GetOutput();
      for (var i = 0; i < output.Length; i += 3) {
        if (output[i] == -1 && output[i + 1] == 0) {
          currentScore = output[i + 2];
        }
        if (output[i + 2] == 4) {
          ball = output[i];
        } else if (output[i + 2] == 3) {
          paddle = output[i];
        }
      }
    }

    return currentScore;
  }
}
