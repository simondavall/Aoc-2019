using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day09;

internal static class Program
{
  private const long ExpectedPartOne = 3638931938;
  private const long ExpectedPartTwo = 86025;

  public static int Main(string[] args)
  {
    Console.WriteLine("\n## Day 9: Sensor Boost ##");
    Console.WriteLine("https://adventofcode.com/2019/day/9");
 
    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      long[] input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne(long[] program)
  {
    var computer = new IntcodeComputer(program);

    while (!computer.IsHalted)
    {
      if (computer.IsAwaitingInput)
      {
        computer.SetInput(1);
      }
      computer.Execute();
    }

    //Console.WriteLine($"Full output: {computer.FullOutput.Print(30)}");

    return computer.GetLastOutput;
  }

  private static long PartTwo(long[] program)
  {
    var computer = new IntcodeComputer(program);

    while (!computer.IsHalted)
    {
      if (computer.IsAwaitingInput)
      {
        computer.SetInput(2);
      }
      computer.Execute();
    }

    //Console.WriteLine($"Full output: {computer.FullOutput.Print(30)}");

    return computer.GetLastOutput;
  }

  private static long[] GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath)){
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader
      .ReadToEnd()
      .Split(',', StringSplitOptions.RemoveEmptyEntries)
      .ToLongArray();

    return data;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
