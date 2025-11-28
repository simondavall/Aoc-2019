using Spacecraft;

namespace Day17;

internal static partial class Program {
  private const string Title = "\n## Day 17: Set and Forget ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/17";
  private const long ExpectedPartOne = 7404;
  private const long ExpectedPartTwo = 929045;

  private static readonly (char ch, int dx, int dy)[] _directions = [ ('^', 0, -1),('>', 1, 0),('v', 0, 1),('<', -1, 0)];

  private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);
    computer.Execute();

    var (grid, width, height, _) = GetGrid(computer.GetOutput());

    bool isIntersection = true;
    long tally = 0;
    for (var y = 0; y < height; y++) {
      for (var x = 0; x < width; x++) {
        foreach (var (_, dx, dy) in _directions) {
          var nx = x + dx;
          var ny = y + dy;
          if (!IsInBounds(nx, ny, width, height) || grid[y][x] != '#' || grid[ny][nx] != '#') {
            isIntersection = false;
            break;
          }
        }
        if (isIntersection) {
          grid[y][x] = 'O';
          tally += isIntersection ? x * y : 0;
        }
        isIntersection = true;
      }
    }

    // Print grid with intersections highlighted
    Console.WriteLine();
    foreach (var r in grid) {
      foreach (var ch in r) {
        Console.Write(ch);
      }
      Console.WriteLine();
    }
    Console.WriteLine();

    return tally;
  }

 private static long PartTwo(long[] program) {
    var computer = new IntcodeComputer(program);
    computer.Execute();

    var (grid, width, height, start) = GetGrid(computer.GetOutput());

    var (dir, x, y) = start;
    var dist = 0;
    var directions = new List<string>();
    while (true) {
      // try straight ahead
      var (_, dx, dy) = _directions[dir];
      var (nx, ny) = (x + dx, y + dy);
      if (IsInBounds(nx, ny, width, height) && grid[ny][nx] == '#'){
        x = nx;
        y = ny;
        dist++;
        continue;
      }

      // try turn left
      var newDir = (dir + 3) % 4;
      (_, dx, dy) = _directions[newDir];
      (nx, ny) = (x + dx, y + dy);

      if (IsInBounds(nx, ny, width, height) && grid[ny][nx] == '#'){
        directions.Add(dist.ToString());
        dist = 0;
        directions.Add("L");
        dir = newDir;
        continue;
      }

      // try turn right
      newDir = (dir + 1) % 4;
      (_, dx, dy) = _directions[newDir];
      (nx, ny) = (x + dx, y + dy);
      
      if (IsInBounds(nx, ny, width, height) && grid[ny][nx] == '#'){
        directions.Add(dist.ToString());
        dist = 0;
        directions.Add("R");
        dir = newDir;
        continue;
      }

      directions.Add(dist.ToString());
      break;
    }

    Console.WriteLine($"\nInstructions: {string.Join(",", directions)}\n");

    program[0] = 2;

    // The following were found by observing the directions output above.

    var main = "A,A,B,C,B,A,C,B,C,A\n";
    var functionA = "L,6,R,12,L,6,L,8,L,8\n";
    var functionB = "L,6,R,12,R,8,L,8\n";
    var functionC = "L,4,L,4,L,6\n";
    var videoFeed = "n\n"; // set this to y and uncomment lines 136-138 to see video output in console
    var instructions = new Queue<long>($"{main}{functionA}{functionB}{functionC}{videoFeed}".ToLongArray());

    computer = new IntcodeComputer(program);
    long[] output = [];
    while (!computer.IsHalted) {
      if (computer.IsAwaitingInput) {
        var value = instructions.Dequeue();
        computer.SetInput(value);
      }
      computer.Execute();
      output = computer.GetOutput();
      // foreach (var ch in output) {
      //   Console.Write((char)ch);
      // }
    }

    return output[^1];
  }

  private static bool IsInBounds(int x, int y, int width, int height) {
    return x >= 0 && x < width && y >= 0 && y < height;
  }
  
  private static (char[][] grid, int width, int height, (int dir, int x, int y)) GetGrid(long[] source) {
    var output = new Queue<long>(source);
    List<char[]> grid = [];
    List<char> row = [];
    (int dir, int x, int y) start = (-1, -1, -1);
    while (output.Count > 0) {
      var ch = (char)output.Dequeue();
      switch (ch) {
        case '\n':
          if (row.Count > 0)
            grid.Add(row.ToArray());
          row.Clear();
          break;
        case '.':
        case '#':
          row.Add(ch);
          break;
        case '^':
        case 'v':
        case '<':
        case '>':
          var (dir, _) = _directions.Index().Single(x => x.Item.ch == ch);
          start = (dir, row.Count, grid.Count);
          row.Add(ch);
          break;
        default:
          Console.Write(ch);
          break;
      }
    }

    var width = grid[0].Length;
    var height = grid.Count;
    return (grid.ToArray(), width, height, start);
  }

  private static long[] ToLongArray(this string str) {
    var output = new long[str.Length];
    for (var i = 0; i < str.Length; i++)
      output[i] = str[i];
    return output;
  }
 }
