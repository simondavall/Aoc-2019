using System.Diagnostics;

namespace Day06;

internal static class Program {
  private const long ExpectedPartOne = 119831;
  private const long ExpectedPartTwo = 322;

  public static int Main(string[] args) {
    Console.WriteLine("\n## Day 6: Universal Orbit Map ##");
    Console.WriteLine("https://adventofcode.com/2019/day/6");

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

  private static long PartOne(Node rootNode) {
    long tally = 0;
    var q = new Queue<(Node node, int count)>([(rootNode, 0)]);
    while (true) {
      if (!q.TryDequeue(out var cur))
        break;

      tally += cur.count;
      foreach (var child in cur.node.Children) {
        q.Enqueue((child, cur.count + 1));
      }
    }

    return tally;
  }

  private static long PartTwo(Node rootNode) {
    long tally;

    // Find number of orbits between you and Santa
    Node? you = null;
    var q1 = new Queue<Node>([rootNode]);
    while (q1.Count > 0) {
      if (!q1.TryDequeue(out var cur))
        break;
      foreach (var child in cur.Children) {
        if (child.Name == "YOU") {
          you = child;
          break;
        }
        q1.Enqueue(child);
      }
    }

    if (you is null)
      throw new ApplicationException("Could not find a node in the tree for YOU");

    (Node? node, int count) parent = (you.Parent, 0);
    var seen = new HashSet<string>();
    var q2 = new Queue<(Node node, int count)>([(you, 0)]);
    while (true) {
      if (!q2.TryDequeue(out var cur)) {
        if (parent.node?.Parent is null)
          throw new ApplicationException("Unable to locate the Santa node (SAN) in the node tree.");

        cur = (parent.node, parent.count);
        parent = (parent.node.Parent, parent.count + 1);
      }

      if (cur.node.Name == "SAN") {
        tally = cur.count - 1;
        break;
      }

      foreach (var child in cur.node.Children) {
        if (!seen.Contains(child.Name)) {
          q2.Enqueue((child, cur.count + 1));
          seen.Add(child.Name);
        }
      }
      seen.Add(cur.node.Name);
    }

    return tally;
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

  public class Node {
    public required string Name { get; init; }
    public Node? Parent { get; init; }
    public List<Node> Children { get; private set; } = [];
  }
}
