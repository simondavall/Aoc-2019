namespace Day01;

internal static partial class Program {
  private const string Title = "\n## Day 1: The Tyranny of the Rocket Equation ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/1";

 private const long ExpectedPartOne = 3226822;
  private const long ExpectedPartTwo = 4837367;

  private static long PartOne(int[] modules) {
    long tally = 0;

    for (int i = 0; i < modules.Length; i++)
      tally += modules[i] / 3 - 2;

    return tally;
  }

  private static long PartTwo(int[] modules) {
    long tally = 0;

    for (int i = 0; i < modules.Length; i++) {
      var m = modules[i];

      while (true) {
        var fuel = m / 3 - 2;
        if (fuel <= 0)
          break;

        tally += fuel;
        m = fuel;
      }
    }

    return tally;
  }
}
