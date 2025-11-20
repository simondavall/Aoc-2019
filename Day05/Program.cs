using AocHelper;
using Spacecraft;

namespace Day05;

internal static partial class Program {
  private const string Title = "\n## Day 5: Sunny with a Chance of Asteroids ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/5";

 private const long ExpectedPartOne = 10987514;
  private const long ExpectedPartTwo = 14195011;

 private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);
    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        computer.SetInput(1);
      }
      computer.Execute();
    }

    Console.WriteLine($"Full output: {computer.FullOutput.Print()}");

    return computer.GetLastOutput;
  }

  private static long PartTwo(long[] program) {
    var computer = new IntcodeComputer(program);
    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        computer.SetInput(5);
      }
      computer.Execute();
    }

    return computer.GetLastOutput;
  }
}
