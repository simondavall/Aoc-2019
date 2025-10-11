namespace ShipComputer;

public class IntcodeProgram
{
  public static void Execute(long[] program)
  {
    var ip = 0;
    long resAddr, val1Addr, val2Addr;

    while (ip < program.Length && program[ip] != 99)
    {
      var opCode = program[ip];
      switch (opCode)
      {
        case 1: // add
          if (ip + 4 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          val1Addr = program[ip + 1];
          val2Addr = program[ip + 2];
          resAddr = program[ip + 3];
          program[resAddr] = program[val1Addr] + program[val2Addr];
          ip += 4;
          break;

        case 2: // multiply
          if (ip + 4 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          val1Addr = program[ip + 1];
          val2Addr = program[ip + 2];
          resAddr = program[ip + 3];
          program[resAddr] = program[val1Addr] * program[val2Addr];
          ip += 4;
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }
}
