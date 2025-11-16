using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day05;

internal static class Program
{
  private const long ExpectedPartOne = 10987514;
  private const long ExpectedPartTwo = 14195011;

  public static int Main(string[] args)
  {
    Console.WriteLine("\n## Day 5: Sunny with a Chance of Asteroids ##");
    Console.WriteLine("https://adventofcode.com/2019/day/5");
 
    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      long[] program = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(program);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(program);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    Console.WriteLine();
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

    Console.WriteLine($"Full output: {computer.FullOutput.Print()}");
    
    return computer.GetLastOutput;
  }

  private static long PartTwo(long[] program)
  {
    var computer = new IntcodeComputer(program);
    while (!computer.IsHalted)
    {
      if (computer.IsAwaitingInput)
      {
        computer.SetInput(5);
      }
      computer.Execute();
    }

    return computer.GetLastOutput;
  }

  private static long[] GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath)){
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    long[] data = streamReader
      .ReadToEnd()
      .Split([',', '\n'], StringSplitOptions.RemoveEmptyEntries)
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
