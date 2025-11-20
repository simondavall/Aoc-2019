using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day11;

internal static class Program {
  private const long ExpectedPartOne = 1885;
  private const long ExpectedPartTwo = 0;

  public static int Main(string[] args) {
    Console.WriteLine("\n## Day 11: Space Police ##");
    Console.WriteLine("https://adventofcode.com/2019/day/11");

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

  private static (int dx, int dy)[] Directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

  private static int ChangeDirection(long dir, int curDir) {
    if (dir == 0) {
      return (4 + curDir - 1) % 4;
    } else if (dir == 1) {
      return (curDir + 1) % 4;
    }
    throw new ApplicationException($"Unknown direction change indicator. Value:'{dir}'");
  }

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
      if (computer.FullOutput.Length >= 2) {
        var output = computer.FullOutput[^2..];
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
      if (computer.FullOutput.Length >= 2) {
        var output = computer.FullOutput[^2..];
        map[(curPos.x, curPos.y)] = output[0]; 
        curDir = ChangeDirection(output[1], curDir);
        (int dx, int dy) = Directions[curDir];
        curPos = (curPos.x + dx, curPos.y + dy);
      }
    }
    
    PrintPanel(map);
    return 0;
  }

  private static void PrintPanel(Dictionary<(int x, int y), long> hullPanels){
    int minX = 0, maxX = 0;
    int minY = 0, maxY = 0;
    foreach(var (x, y) in hullPanels.Keys){
      minX = Math.Min(minX, x);
      maxX = Math.Max(maxX, x);
      minY = Math.Min(minY, y);
      maxY = Math.Max(maxY, y);
    } 

    for (var y = minY - 1; y <= maxY + 1; y++){
      for(var x = minX - 1; x <= maxX + 1; x++){
        hullPanels.TryGetValue((x, y), out long p);
        Console.Write(p == 0 ? "..": "##");
      }
      Console.WriteLine();
    }
  }
  

  private static long[] GetData(string filePath) {
    if (string.IsNullOrWhiteSpace(filePath)) {
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split(',', StringSplitOptions.RemoveEmptyEntries).ToLongArray();

    return data;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
