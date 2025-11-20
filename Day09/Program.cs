using Spacecraft;

namespace Day09;

internal static partial class Program {
  private const string Title = "\n## Day 9: Sensor Boost ##";
  private const string AdventOfCodeUrl = "https://adventofcode.com/2019/day/9";

  private const long ExpectedPartOne = 3638931938;
  private const long ExpectedPartTwo = 86025;

  private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);

    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        computer.SetInput(1);
      }
      computer.Execute();
    }

    return computer.GetLastOutput;
  }

  private static long PartTwo(long[] program) {
    var computer = new IntcodeComputer(program);

    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        computer.SetInput(2);
      }
      computer.Execute();
    }

    return computer.GetLastOutput;
  }
}
