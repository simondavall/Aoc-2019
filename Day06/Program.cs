using System.Diagnostics;

namespace Day06;

internal static class Program
{
  private const long ExpectedPartOne = 119831;
  private const long ExpectedPartTwo = 322;

  public static int Main(string[] args)
  {
    long resultPartOne = -1;
    long resultPartTwo = -1;

    PrintTitle();
    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      Node input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne(Node com)
  {
    long tally = 0;
    var q = new Queue<(Node node, int count)>([(com, 0)]);
    while (true)
    {
      if (!q.TryDequeue(out var cur))
        break;

      tally += cur.count;
      foreach (var child in cur.node.Children)
      {
        q.Enqueue((child, cur.count + 1));
      }
    }

    return tally;
  }

  private static long PartTwo(Node com)
  {
    long tally;

    Node? you = null;
    var qU = new Queue<Node>([com]);
    while (true){
      if (!qU.TryDequeue(out var cur))
        break;
      foreach(var child in cur.Children){
        if (child.Name == "YOU"){
          you = child;
          break;
        }
        qU.Enqueue(child);
      }  
    }

    if (you is null)
      Debug.Assert(you is not null, "Could not find a node in the tree for YOU");

    (Node? node, int count) parent = (you.Parent, 0);
    var seen = new HashSet<string>();
    var q = new Queue<(Node node, int count)>([(you, 0)]);
    while (true){
      if (!q.TryDequeue(out var cur)){
        if (parent.node?.Parent is null){
          Debug.Fail("Unable to locate the Santa node (SAN) in the node tree.");
        }
        
        cur = (parent.node, parent.count);
        parent = (parent.node.Parent, parent.count + 1);
      }
 
      if (cur.node.Name == "SAN"){
        tally = cur.count - 1;
        break;
      }

      foreach(var child in cur.node.Children){
        if (!seen.Contains(child.Name)){
          q.Enqueue((child, cur.count + 1));
          seen.Add(child.Name);
        }
      }
      seen.Add(cur.node.Name);
    }

    return tally;
  }

  private static Node GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath)){
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    Node rootNode = null!;
    var q = new Queue<(string name, Node? node)>([("COM", null)]);

    while (true)
    {
      if (!q.TryDequeue(out var cur))
        break;

      Node? activeNode = null;

      if (cur.node is null)
      {
        activeNode = new Node { Name = cur.name };
        rootNode = activeNode;
      }
      else
      {
        activeNode = new Node { Name = cur.name, Parent = cur.node };
        cur.node.Children.Add(activeNode);
      }

      foreach (var item in data.Where(x => x.StartsWith(cur.name)).Select(x => x[(x.IndexOf(')') + 1)..]))
      {
        q.Enqueue((item, activeNode));
      }
    }

    return rootNode;
  }

  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 6: Universal Orbit Map ##");
    Console.WriteLine("https://adventofcode.com/2019/day/6");
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }

  public class Node
  {
    public required string Name { get; init; }
    public Node? Parent { get; init; }
    public List<Node> Children { get; private set; } = [];
  }
}
