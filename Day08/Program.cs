using System.Diagnostics;

namespace Day08;

internal static class Program
{
  private const long ExpectedPartOne = 2016;
  private const long ExpectedPartTwo = 0;

  public static int Main(string[] args)
  {
    long resultPartOne = -1;
    long resultPartTwo = -1;

    PrintTitle();
    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      string input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
 
    // PrintTitle();
    // var input = GetData(args);
    // var stopwatch = Stopwatch.StartNew();
    //
    // var resultPartOne = PartOne(input);
    // PrintResult("1", resultPartOne.ToString(), stopwatch);
    //
    // var resultPartTwo = PartTwo(input);
    // PrintResult("2", resultPartTwo.ToString(), stopwatch);
    //
    // return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static long PartOne(ReadOnlySpan<char> image)
  {
    int minZeroCount = int.MaxValue;
    ReadOnlySpan<char> minLayer = [];

    var layerSize = 25 * 6;
   
    int idx = 0;
    while (idx < image.Length - 1){
      var layer = image.Slice(idx, layerSize);
      var count = layer.Count('0');
      if (count < minZeroCount){
        minZeroCount = count;
        minLayer = layer;     
      }
      idx += layerSize;
    }

    return minLayer.Count('1') * minLayer.Count('2');
  }

  private static long PartTwo(ReadOnlySpan<char> image)
  {
    int width = 25, height = 6;
    var layerSize = width * height;
    var finalImage = new char[layerSize];

    for (int pt = 0; pt < layerSize; pt++){
      int idx = 0;
      while (idx < image.Length - 1){
        char pixel = image[idx + pt];
        if (pixel != '2'){
          finalImage[pt] = pixel;
          break;
        }
        idx += layerSize;
      }
    }

    PrintImage(width, height, finalImage);

    return 0;
  }

  private static void PrintImage(int width, int height, char[] image){
    Console.WriteLine();
    for(int i = 0; i < height; i++){
      for (int j = 0; j < width; j++){
        Console.Write(image[i * width + j] == '1' ? "##" : "  "  );
      }
      Console.WriteLine();
    }
    Console.WriteLine();
  }

  private static string GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath)){
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd();

    return data;
  }

  private static void PrintTitle()
  {
    Console.WriteLine("# Advent of Code 2019 #");
    Console.WriteLine("## Day 8: Space Image Format ##");
    Console.WriteLine("https://adventofcode.com/2019/day/8");
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
