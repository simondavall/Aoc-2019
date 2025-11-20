namespace Day03;

internal static partial class Program {
  private const string Title = "\n## Day 3: Crossed Wires ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/3";

  private const long ExpectedPartOne = 1431;
  private const long ExpectedPartTwo = 48012;

  private static readonly Dictionary<char, (int dr, int dc)> Directions = new()
  {{ 'U', (-1, 0) },
    { 'R', (0, 1) },
    { 'D', (1, 0) },
    { 'L', (0, -1) } };

  private static long PartOne((char, int)[][] wires) {
    long minDist = int.MaxValue;

    var visited = new HashSet<(int, int)>();

    // first wire path
    int r = 0, c = 0;
    foreach (var (dir, val) in wires[0]) {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++) {
        r += dr;
        c += dc;
        visited.Add((r, c));
      }
    }

    // compare second wire path with first
    r = 0;
    c = 0;
    foreach (var (dir, val) in wires[1]) {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++) {
        r += dr;
        c += dc;
        if (visited.Contains((r, c))) {
          var dist = Math.Abs(r) + Math.Abs(c);
          if (minDist > dist)
            minDist = dist;
        }
      }
    }

    return minDist;
  }

  private static long PartTwo((char, int)[][] wires) {
    long minDist = int.MaxValue;

    var visited = new Dictionary<(int, int), int>();

    // first wire path
    int r = 0, c = 0, steps = 0;
    foreach (var (dir, val) in wires[0]) {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++) {
        steps++;
        r += dr;
        c += dc;
        if (!visited.ContainsKey((r, c)))
          visited.Add((r, c), steps);
      }
    }

    // compare second wire path with first
    r = 0;
    c = 0;
    steps = 0;
    foreach (var (dir, val) in wires[1]) {
      var (dr, dc) = Directions[dir];
      for (var i = 0; i < val; i++) {
        steps++;
        r += dr;
        c += dc;
        if (visited.ContainsKey((r, c))) {
          var dist = steps + visited[(r, c)];
          if (minDist > dist)
            minDist = dist;
        }
      }
    }

    return minDist;
  }
}
