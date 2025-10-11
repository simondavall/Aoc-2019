using System.Diagnostics;
using AocHelper;
using ShipComputer;

namespace Day02;

internal static class Program
{
  private const long ExpectedPartOne = 0;
  private const long ExpectedPartTwo = 0;
  private static long[] _program = [];

  public static int Main(string[] args)
  {
    PrintTitle();

    SetData(args);

    var stopwatch = Stopwatch.StartNew();

    var resultPartOne = PartOne();
    PrintResult("1", resultPartOne.ToString(), stopwatch);

    var resultPartTwo = PartTwo();
    PrintResult("2", resultPartTwo.ToString(), stopwatch);

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne()
  {
    long[] programCopy = new long[_program.Length];
    Array.Copy(_program, programCopy, _program.Length);

    // Remove the following reset if running the sample data.
    programCopy[1] = 12;
    programCopy[2] = 2;

    IntcodeProgram.Execute(programCopy);

    return programCopy[0];
  }

  private static long PartTwo()
  {
    long result = 0;
    long[] programCopy = new long[_program.Length];

    bool terminated = false;
    for (int noun = 0; noun < 100; noun++){
      for (int verb = 0; verb < 100; verb++){
        Array.Copy(_program, programCopy, _program.Length);
        programCopy[1] = noun;
        programCopy[2] = verb;
        IntcodeProgram.Execute(programCopy);
        if (programCopy[0] == 19690720){
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

  private static void SetData(string[] args)
  {
    var filename = "Day02/sample.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    _program = streamReader.ReadToEnd().Split([',', '\n'], StringSplitOptions.RemoveEmptyEntries).ToLongArray();
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
