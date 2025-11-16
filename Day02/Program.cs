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
    Console.WriteLine("\n## Day 2: 1202 Program Alarm ##");
    Console.WriteLine("https://adventofcode.com/2019/day/2");
 
    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      long[] input = GetData(filePath).ToLongArray();
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
    var computer = new  IntcodeComputer(program);
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

  private static string[] GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath)){
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader
      .ReadToEnd()
      .Split(',', StringSplitOptions.RemoveEmptyEntries);

    return data;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
