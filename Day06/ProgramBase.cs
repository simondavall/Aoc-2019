using System.Diagnostics;

namespace Day06;

internal static partial class Program {
  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      Node rootNode = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(rootNode);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(rootNode);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

   private static Node GetData(string filePath) {
    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    Node rootNode = null!;
    var q = new Queue<(string name, Node? node)>([("COM", null)]);

    while (true) {
      if (!q.TryDequeue(out var cur))
        break;

      Node? newNode = null;

      if (cur.node is null) {
        newNode = new Node { Name = cur.name };
        rootNode = newNode;
      } else {
        newNode = new Node { Name = cur.name, Parent = cur.node };
        cur.node.Children.Add(newNode);
      }

      foreach (var item in data.Where(x => x.StartsWith(cur.name)).Select(x => x[(x.IndexOf(')') + 1)..])) {
        q.Enqueue((item, newNode));
      }
    }

    return rootNode;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
 } 
