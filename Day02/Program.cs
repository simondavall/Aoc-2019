using System.Diagnostics;
using AocHelper;
using Spacecraft;

namespace Day02;

internal static class Program
{
  private const long ExpectedPartOne = 6568671;
  private const long ExpectedPartTwo = 3951;

  public static int Main(string[] args)
  {
    PrintTitle();

    var program = GetData(args);

    var stopwatch = Stopwatch.StartNew();

    var resultPartOne = PartOne(program);
    PrintResult("1", resultPartOne.ToString(), stopwatch);

    var resultPartTwo = PartTwo(program);
    PrintResult("2", resultPartTwo.ToString(), stopwatch);

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne(long[] program)
  {
    var computer = new IntcodeComputer(program);
    computer.SetMemory(1, 12);
    computer.SetMemory(2, 2);
    while(!computer.IsHalted){
      if (computer.IsAwaitingInput)
        computer.SetInput(0);

      computer.Execute();
    }

    return computer.ReadMemory(0);
  }

  private static long PartTwo(long[] program)
  {
    long result = 0;
    IntcodeComputer computer; 

    bool terminated = false;
    for (int noun = 0; noun < 100; noun++){
      for (int verb = 0; verb < 100; verb++){
        computer = new IntcodeComputer(program);
        computer.SetMemory(1, noun);
        computer.SetMemory(2, verb);
        while(!computer.IsHalted){
          if (computer.IsAwaitingInput)
            computer.SetInput(0);

          computer.Execute();
        }

        if (computer.ReadMemory(0) == 19690720){
          terminated = true;
          result = 100 * noun + verb;
          break;
        }
      }
      if (terminated)
        break;
    }

    if (!terminated)
      throw new ApplicationException("Correct value was not found. Program terminated.");

    return result;
  }

  private static long[] GetData(string[] args)
  {
    var filename = "sample.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    var data = streamReader.ReadToEnd().Split([',', '\n'], StringSplitOptions.RemoveEmptyEntries).ToLongArray();

    return data;
  }

  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 2: 1202 Program Alarm ##");
    Console.WriteLine("https://adventofcode.com/2019/day/2");
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
