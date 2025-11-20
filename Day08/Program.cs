namespace Day08;

internal static partial class Program {
  private const string Title = "\n## Day 8: Space Image Format ##";
  private const string AdventOfCodeUrl = "https://adventofcode.com/2019/day/8";

  private const long ExpectedPartOne = 2016;
  private const long ExpectedPartTwo = 0;

  private static long PartOne(ReadOnlySpan<char> image) {
    int minZeroCount = int.MaxValue;
    ReadOnlySpan<char> minLayer = [];

    var layerSize = 25 * 6;

    int idx = 0;
    while (idx < image.Length - 1) {
      var layer = image.Slice(idx, layerSize);
      var count = layer.Count('0');
      if (count < minZeroCount) {
        minZeroCount = count;
        minLayer = layer;
      }
      idx += layerSize;
    }

    return minLayer.Count('1') * minLayer.Count('2');
  }

  private static long PartTwo(ReadOnlySpan<char> image) {
    int width = 25, height = 6;
    var layerSize = width * height;
    var finalImage = new char[layerSize];

    for (int pt = 0; pt < layerSize; pt++) {
      int idx = 0;
      while (idx < image.Length - 1) {
        char pixel = image[idx + pt];
        if (pixel != '2') {
          finalImage[pt] = pixel;
          break;
        }
        idx += layerSize;
      }
    }

    PrintImage(width, height, finalImage);

    return 0;
  }

  private static void PrintImage(int width, int height, char[] image) {
    Console.WriteLine();
    for (int i = 0; i < height; i++) {
      for (int j = 0; j < width; j++) {
        Console.Write(image[i * width + j] == '1' ? "##" : "  ");
      }
      Console.WriteLine();
    }
    Console.WriteLine();
  }
}
