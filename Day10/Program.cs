using System.Diagnostics;


namespace Day10;

internal static class Program
{
  private const long ExpectedPartOne = 227;
  private const long ExpectedPartTwo = 604;

  private static long PartOne(string[] data)
  {
    var asteroids = GetAsteroids(data);
    var sightLines = new HashSet<(int x, int y)>[asteroids.Length];
    for (var i = 0; i < asteroids.Length; i++)
    {
      sightLines[i] = new HashSet<(int x, int y)>();
      for (var j = 0; j < asteroids.Length; j++)
      {
        if (i == j) continue;

        var (x, y, _) = asteroids[i].GetSightLineTo(asteroids[j]);
        sightLines[i].Add((x, y));
      }
      asteroids[i].SightLines = sightLines[i].Count;
    }

    int maxSightLines = 0;
    foreach (var asteroid in asteroids)
    {
      if (asteroid.SightLines > maxSightLines)
      {
        maxSightLines = asteroid.SightLines;
        bestPlacedAsteroid = asteroid;
      }
    }

    return maxSightLines;
  }

  private static Asteroid bestPlacedAsteroid;

  private static long PartTwo(string[] data)
  {
    var asteroids = GetAsteroids(data).ToList();
    if (asteroids.Count < 200)
    {
      return -1; // requires a test with at least 200 asteroids
    }

    int vapourizedCount = 0;
    while (asteroids.Count > 0)
    {
      var sightLines = new Dictionary<(int x, int y), (int lcd, Asteroid asteroid)>();
      for (var i = asteroids.Count - 1; i >= 0; i--)
      {
        var (x, y, lcd) = bestPlacedAsteroid.GetSightLineTo(asteroids[i]);
        if ((x, y) == (0, 0))
        {
          asteroids.RemoveAt(i);
          continue;
        }
        if (!sightLines.TryGetValue((x, y), out var value))
        {
          sightLines[(x, y)] = (lcd, asteroids[i]);
          asteroids.RemoveAt(i);
        }
        else if (lcd < value.lcd)
        {
          asteroids.Add(value.asteroid);
          sightLines[(x, y)] = (lcd, asteroids[i]);
          asteroids.RemoveAt(i);
        }
      }
      var currentTargets = sightLines.Keys.ToList();
      var rotationalSort = new RotationalComparer();
      currentTargets.Sort(rotationalSort);
      foreach (var (x, y) in currentTargets)
      {
        vapourizedCount += 1;
        if (vapourizedCount == 200)
        {
          var (_, asteroid) = sightLines[(x, y)];
          return asteroid.X * 100 + asteroid.Y;
        }
      }
    }

    return -1;
  }

  internal struct Asteroid(int x, int y)
  {
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int SightLines { get; set; }

    public readonly (int x, int y, int lcd) GetSightLineTo(Asteroid other)
    {
      var (diff_x, diff_y) = (other.X - X, other.Y - Y);
      var lcd = LargestCommonDivisor(diff_x, diff_y);

      return lcd == 0 ? (diff_x, diff_y, 0) : (diff_x / lcd, diff_y / lcd, lcd);
    }

    public readonly override string ToString()
    {
      return $"X:{X}, Y:{Y}";
    }

    private static int LargestCommonDivisor(int a, int b)
    {
      while (b != 0)
      {
        int temp = b;
        b = a % b;
        a = temp;
      }
      return Math.Abs(a);
    }
  }

  internal class RotationalComparer : IComparer<(int x, int y)>
  {
    public int Compare((int x, int y) a, (int x, int y) b)
    {
      var aTanA = Math.Atan2(a.x, a.y);
      var aTanB = Math.Atan2(b.x, b.y);
      return aTanB.CompareTo(aTanA);
    }
  }

  private static Asteroid[] GetAsteroids(string[] data)
  {
    List<Asteroid> asteroids = [];
    for (var y = 0; y < data.Length; y++)
    {
      for (var x = 0; x < data[y].Length; x++)
      {
        if (data[y][x] == '#')
        {
          asteroids.Add(new Asteroid(x, y));
        }
      }
    }
    return asteroids.ToArray();
  }

  public static int Main(string[] args)
  {
    Console.WriteLine("\n## Day 10: Monitoring Station ##");
    Console.WriteLine("https://adventofcode.com/2019/day/10");

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args)
    {
      Console.WriteLine($"\nFile: {filePath}\n");
      string[] input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static string[] GetData(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath))
    {
      filePath = "sample.txt";
    }

    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);

    return data;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
