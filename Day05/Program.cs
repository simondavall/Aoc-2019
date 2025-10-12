using System.Diagnostics;
using AocHelper;
using ShipComputer;

namespace Day05;

internal static class Program
{
  private const long ExpectedPartOne = 10987514;
  private const long ExpectedPartTwo = 14195011;
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

    IntcodeProgram.Execute(programCopy, 1);

    return IntcodeProgram.DiagnosticCode;
  }

  private static long PartTwo()
  {
    long[] programCopy = new long[_program.Length];
    Array.Copy(_program, programCopy, _program.Length);

    IntcodeProgram.Execute(programCopy, 5);

    return IntcodeProgram.DiagnosticCode;
  }

  private static void SetData(string[] args)
  {
    var filename = "sample.txt";
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
      filename = args[0];

    using var streamReader = new StreamReader(filename);
    _program = streamReader.ReadToEnd().Split([',', '\n'], StringSplitOptions.RemoveEmptyEntries).ToLongArray();
  }


  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 5: Sunny with a Chance of Asteroids ##");
    Console.WriteLine("https://adventofcode.com/2019/day/5");
    Console.WriteLine();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine();
    Console.WriteLine($"Part {partNo}\\");
    Console.WriteLine($"Result: {result}\\");
    Console.WriteLine($"Time elapsed (ms): {sw.Elapsed.TotalMilliseconds}");
    sw.Restart();
  }
}
