using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day11;

internal static partial class Program {
  private const string Title = "\n## Day 11: Space Police ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/11";

  private const long ExpectedPartOne = 1885;
  private const long ExpectedPartTwo = 0;

  private static (int dx, int dy)[] Directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

  private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);
    var map = new Dictionary<(int x, int y), long>();
    (int x, int y) curPos = (0, 0);
    var curDir = 0;

    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        map.TryGetValue(curPos, out var curInput);
        computer.SetInput(curInput);
      }
      computer.Execute();
      var output = computer.GetOutput();
      if (output.Length == 2) {
        map[(curPos.x, curPos.y)] = output[0];
        curDir = ChangeDirection(output[1], curDir);
        (int dx, int dy) = Directions[curDir];
        curPos = (curPos.x + dx, curPos.y + dy);
      }
    }

    return map.Keys.Count;
  }

  private static long PartTwo(long[] program) {
    var computer = new IntcodeComputer(program);
    var map = new Dictionary<(int x, int y), long>();
    (int x, int y) curPos = (0, 0);
    map[curPos] = 1; // start with a white panel
    var curDir = 0;

    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        map.TryGetValue(curPos, out long curInput);
        computer.SetInput(curInput);
      }
      computer.Execute();
      var output = computer.GetOutput();

      if (output.Length == 2) {
        map[(curPos.x, curPos.y)] = output[0];
        curDir = ChangeDirection(output[1], curDir);
        (int dx, int dy) = Directions[curDir];
        curPos = (curPos.x + dx, curPos.y + dy);
      }
    }

    PrintPanel(map);
    return 0;
  }

  private static int ChangeDirection(long dir, int curDir) {
    if (dir == 0) {
      return (4 + curDir - 1) % 4;
    } else if (dir == 1) {
      return (curDir + 1) % 4;
    }
    throw new ApplicationException($"Unknown direction change indicator. Value:'{dir}'");
  }

  private static void PrintPanel(Dictionary<(int x, int y), long> hullPanels) {
    int minX = int.MaxValue, maxX = 0;
    int minY = int.MaxValue, maxY = 0;
    foreach (var (x, y) in hullPanels.Keys) {
      minX = Math.Min(minX, x);
      maxX = Math.Max(maxX, x);
      minY = Math.Min(minY, y);
      maxY = Math.Max(maxY, y);
    }

    for (var y = minY - 1; y <= maxY + 1; y++) {
      for (var x = minX - 1; x <= maxX + 1; x++) {
        hullPanels.TryGetValue((x, y), out long p);
        Console.Write(p == 1 ? "##" : "  ");
      }
      Console.WriteLine();
    }
  }
}
