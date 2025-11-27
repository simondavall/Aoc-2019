namespace Day16;

internal static partial class Program {
  private const string Title = "\n## Day 16: Flawed Frequency Transmission ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/16";
  private const string ExpectedPartOne = "82525123";
  private const string ExpectedPartTwo = "49476260";

  private static readonly int[] _basePattern = new int[] { 0, 1, 0, -1 };

  public static string PartOne(int[] signal) {
    int phases = 100;
    Dictionary<int, int> output = [];
    for (int i = 0; i < signal.Length; i++) {
      output[i] = signal[i];
    }

    while (phases-- > 0) {
      int result = 0;
      var current = new Dictionary<int, int>(output);

      for (int i = signal.Length - 1; i >= 0; i--) {
        if (i < signal.Length / 2) {
          var pattern = GetPattern(i + 1, signal.Length);
          result = 0;
          for (int j = 0; j < signal.Length; j++)
            result += current[j] * pattern[j];
        } else {
          result += current[i];
        }
        output[i] = Math.Abs(result % 10);
      }
    }

    return string.Join("", output.Values.Take(8));
  }

  public static string PartTwo(int[] signal) {
    int phases = 100;

    int extLength = signal.Length * 10_000;
    int[] extSignal = new int[extLength];
    for (int i = 0; i < extLength; i++) {
      extSignal[i] = signal[i % signal.Length];
    }

    int offset = 0;
    for (int i = 0; i < 7; i++) {
      offset *= 10;
      offset += signal[i];
    }

    Dictionary<int, int> output = [];
    for (int i = offset; i < extLength; i++)
      output[i] = extSignal[i];

    if (extLength > offset) {
      while (phases-- > 0) {
        int result = 0;
        var current = output.ToDictionary();
        for (int i = extLength - 1; i >= offset; i--) {
          if (i < extLength / 2) {
            var pattern = GetPattern(i + 1, extLength);
            result = 0;
            for (int j = 0; j < extLength; j++)
              result += current[j] * pattern[j];
          } else {
            result += current[i];
          }
          output[i] = Math.Abs(result % 10);
        }
      }
    }

    return string.Join("", output.Values.Take(8));
  }

  private static Dictionary<(int, int), Dictionary<int, int>> cachedPatterns = [];

  private static Dictionary<int, int> GetPattern(int position, int signalLength) {
    if (cachedPatterns.TryGetValue((position, signalLength), out var cachedValue)){
      return cachedValue;
    }
    var pattern = new Dictionary<int, int>();

    int index = 0, baseIndex = 0;
    while (index <= signalLength) {
      for (int i = 0; i < position; i++) {
        if (index > signalLength){
          break;
        }

        if (index - 1 >= 0)
          pattern[index - 1] = _basePattern[baseIndex];

        index++;
      }

      baseIndex = ++baseIndex % _basePattern.Length;
    }
    cachedPatterns[(position, signalLength)] = pattern;
    return pattern;
  }
}
