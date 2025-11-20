namespace Day12;

internal static partial class Program {
  private const string Title = "\n## Day 12: The N-Body Problem ##";
  private const string AdventOfCodde = "https://adventofcode.com/2019/day/12";

  private const long ExpectedPartOne = 7471;
  private const long ExpectedPartTwo = 376243355967784;

  // This holds the number of steps to take for each file imported.
  private static Queue<int> _totalSteps = new Queue<int>([10, 100, 1000]);

  private static long PartOne(Moon[] moons) {
    var step = 0;
    var totalSteps = _totalSteps.Dequeue();
    while (step++ < totalSteps) {
      List<Moon> newMoons = [];
      foreach (var moon in moons) {
        newMoons.Add(moon.Copy());
      }
      // update velocities
      for (var i = 0; i < moons.Length; i++) {
        for (var j = 0; j < moons.Length; j++) {
          if (i == j) continue;
          newMoons[i].velocity.dx -= moons[i].position.x.CompareTo(moons[j].position.x);
          newMoons[i].velocity.dy -= moons[i].position.y.CompareTo(moons[j].position.y);
          newMoons[i].velocity.dz -= moons[i].position.z.CompareTo(moons[j].position.z);
        }
      }
      // update postions
      foreach (var moon in newMoons) {
        moon.position.x += moon.velocity.dx;
        moon.position.y += moon.velocity.dy;
        moon.position.z += moon.velocity.dz;
      }
      moons = newMoons.ToArray();
    }

    long totalEnergy = 0;

    foreach (var moon in moons) {
      var potential = Math.Abs(moon.position.x) + Math.Abs(moon.position.y) + Math.Abs(moon.position.z);
      var kinetic = Math.Abs(moon.velocity.dx) + Math.Abs(moon.velocity.dy) + Math.Abs(moon.velocity.dz);
      totalEnergy += potential * kinetic;
    }

    return totalEnergy;
  }

  private static long PartTwo(Moon[] moons) {
    List<Moon> origMoons = [];
    Array.ForEach(moons, x => origMoons.Add(x.Copy()));

    var step = 0;
    bool foundX = false, foundY = false, foundZ = false;
    long frequencyX = 0, frequencyY = 0, frequencyZ = 0;

    while (true) {
      step++;
      List<Moon> newMoons = [];
      foreach (var moon in moons) {
        newMoons.Add(moon.Copy());
      }

      for (var i = 0; i < moons.Length; i++) {
        for (var j = 0; j < moons.Length; j++) {
          if (i == j) continue;
          newMoons[i].velocity.dx -= moons[i].position.x.CompareTo(moons[j].position.x);
          newMoons[i].velocity.dy -= moons[i].position.y.CompareTo(moons[j].position.y);
          newMoons[i].velocity.dz -= moons[i].position.z.CompareTo(moons[j].position.z);
        }
      }
      foreach (var moon in newMoons) {
        moon.position.x += moon.velocity.dx;
        moon.position.y += moon.velocity.dy;
        moon.position.z += moon.velocity.dz;
      }

      moons = newMoons.ToArray();
      bool sameX = true, sameY = true, sameZ = true;
      for (var i = 0; i < moons.Length; i++) {
        if (!foundX && sameX && !moons[i].IsMatchInX(origMoons[i])) {
          sameX = false;
        }
        if (!foundY && sameY && !moons[i].IsMatchInY(origMoons[i])) {
          sameY = false;
        }
        if (!foundZ && sameZ && !moons[i].IsMatchInZ(origMoons[i])) {
          sameZ = false;
        }
      }
      if (!foundX && sameX) {
        foundX = true;
        frequencyX = step;
      }
      if (!foundY && sameY) {
        foundY = true;
        frequencyY = step;
      }
      if (!foundZ && sameZ) {
        foundZ = true;
        frequencyZ = step;
      }
      if (foundX && foundY && foundZ) {
        break;
      }
    }

    var lcd = LargestCommonDivisor(frequencyX, frequencyY);
    var frequencyXY = frequencyX * frequencyY / lcd;
    lcd = LargestCommonDivisor(frequencyXY, frequencyZ);

    return frequencyXY * frequencyZ / lcd;
  }

  private static long LargestCommonDivisor(long a, long b) {
    while (b != 0) {
      long temp = b;
      b = a % b;
      a = temp;
    }
    return Math.Abs(a);
  }

  private class Moon(int x, int y, int z) {
    internal (int x, int y, int z) position = (x, y, z);
    internal (int dx, int dy, int dz) velocity = (0, 0, 0);

    internal bool IsMatchInX(Moon other) {
      return position.x == other.position.x && velocity.dx == other.velocity.dx;
    }
    internal bool IsMatchInY(Moon other) {
      return position.y == other.position.y && velocity.dy == other.velocity.dy;
    }
    internal bool IsMatchInZ(Moon other) {
      return position.z == other.position.z && velocity.dz == other.velocity.dz;
    }

    internal Moon Copy() {
      var newMoon = new Moon(position.x, position.y, position.z);
      newMoon.velocity.dx = velocity.dx;
      newMoon.velocity.dy = velocity.dy;
      newMoon.velocity.dz = velocity.dz;
      return newMoon;
    }
    public override string ToString() {
      return $"position: {position}, velocity:{velocity}";
    }
  }
}
