using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day09;

internal static class Program
{
  private const long ExpectedPartOne = 3638931938;
  private const long ExpectedPartTwo = 0;

  public static int Main(string[] args)
  {
    PrintTitle();
    var input = GetData(args);
    var stopwatch = Stopwatch.StartNew();

    var resultPartOne = PartOne(input);
    PrintResult("1", resultPartOne.ToString(), stopwatch);

    var resultPartTwo = PartTwo();
    PrintResult("2", resultPartTwo.ToString(), stopwatch);

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne(long[] program)
  {
    var computer = new IntcodeComputer(program);

    while (!computer.IsHalted){
      if (computer.IsAwaitingInput){
        computer.SetInput(1);
      }
      computer.Execute();
    }

    Console.WriteLine($"Full output: {computer.FullOutput.Print(30)}");

    return computer.GetLastOutput;
  }

  private static long PartTwo()
  {
    long tally = 0;

    return tally;
  }

  private static long[] GetData(string[] args)
  {
    var filename = "Day09/inputDay09.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    var data = streamReader.ReadToEnd().Split(',', StringSplitOptions.RemoveEmptyEntries).ToLongArray();

    return data;
  }


  private static void PrintTitle(){
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 9: Sensor Boost ##");
    Console.WriteLine("https://adventofcode.com/2019/day/9");
    Console.WriteLine();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo}\\");
    Console.WriteLine($"Result: {result}\\");
    Console.WriteLine($"Time elapsed (ms): {sw.Elapsed.TotalMilliseconds}");
    Console.WriteLine();
    sw.Restart();
  }
}
