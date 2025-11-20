namespace Day06;

internal static partial class Program {
  private const string Title = "\n## Day 6: Universal Orbit Map ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/6";

  private const long ExpectedPartOne = 119831;
  private const long ExpectedPartTwo = 322;

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

  public class Node {
    public required string Name { get; init; }
    public Node? Parent { get; init; }
    public List<Node> Children { get; private set; } = [];
  }
}
