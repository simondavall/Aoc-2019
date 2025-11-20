namespace Day04;

internal static partial class Program {
  private const string Title = "\n## Day 4: Secure Container ##";
  private const string AdventOfCode = "https://adventofcode.com/2019/day/4";

  private const long ExpectedPartOne = 2050;
  private const long ExpectedPartTwo = 1390;

  private static long PartOne() {
    (int min, int max) = (128392, 643281);

    int validPasswordCount = 0;

    for (var i = min; i <= max; i++) {
      var pwd = i.ToDigitArray();
      if (!HasTwoAdjacentNumbers(pwd))
        continue;
      if (!IsNeverDecreasing(pwd))
        continue;

      validPasswordCount++;
    }

    return validPasswordCount;
  }

  private static long PartTwo() {
    (int min, int max) = (128392, 643281);

    int validPasswordCount = 0;

    for (var i = min; i <= max; i++) {
      var pwd = i.ToDigitArray();
      if (!HasOnlyTwoAdjacentNumbers(pwd))
        continue;
      if (!IsNeverDecreasing(pwd))
        continue;

      validPasswordCount++;
    }

    return validPasswordCount;
  }

  private static int[] ToDigitArray(this int n) {
    var arr = new int[NumDigits(n)];
    for (int i = arr.Length - 1; i >= 0; i--) {
      arr[i] = n % 10;
      n /= 10;
    }
    return arr;
  }

  private static bool HasOnlyTwoAdjacentNumbers(int[] code) {
    int[] workingArr = [-1, .. code, -1];
    for (var i = 1; i < code.Length; i++) {
      var cur1 = workingArr[i];
      if (cur1 == workingArr[i + 1]
          && cur1 != workingArr[i - 1]
          && cur1 != workingArr[i + 2])
        return true;
    }
    return false;
  }

  private static bool HasTwoAdjacentNumbers(int[] code) {
    for (var i = 0; i < code.Length - 1; i++) {
      if (code[i] == code[i + 1])
        return true;
    }
    return false;
  }

  private static bool IsNeverDecreasing(int[] code) {
    for (var i = 0; i < code.Length - 1; i++) {
      if (code[i] > code[i + 1])
        return false;
    }
    return true;

  }

  private static int NumDigits(int n) {
    if (n < 0) {
      n = (n == int.MinValue) ? int.MaxValue : -n;
    }
    if (n < 10) return 1;
    if (n < 100) return 2;
    if (n < 1000) return 3;
    if (n < 10000) return 4;
    if (n < 100000) return 5;
    if (n < 1000000) return 6;
    if (n < 10000000) return 7;
    if (n < 100000000) return 8;
    if (n < 1000000000) return 9;
    return 10;
  }
}
