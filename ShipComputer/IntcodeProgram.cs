using System.Diagnostics;

namespace ShipComputer;

public class IntcodeProgram
{
  // Set _verbose to true to activate the console log oputput diagnostics
  private static readonly bool _verbose = false;

  public static long DiagnosticCode { get; private set; }

  public static void Execute(long[] program, int input = 0)
  {
    var ip = 0;
    long val1, val2;

    while (ip < program.Length && program[ip] != 99)
    {
      var (modes, opCode) = GetOpCode(ip, program);
      switch (opCode)
      {
        case 1: // add
          Log("--- Add ---");
          if (ip + 4 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          Debug.Assert(modes[2] == 0, $"For a valid addition call, modes[2] must be zero. Value:'{modes[2]}'");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Log($"val1: {val1}");
          Log($"Mode: {modes[1]}, Program[ip + 2]: {program[ip + 2]} ");
          val2 = modes[1] == 0 ? program[program[ip + 2]] : program[ip + 2];
          Log($"val2: {val2}");
          Log($"Program[ip + 3]: {program[ip + 3]}, Addition:{val1 + val2} ");
          Log($"Setting program[{program[ip + 3]}] to {val1 + val2}");
          program[program[ip + 3]] = val1 + val2;
          ip += 4;
          Log($"New ip:{ip}");
          break;

        case 2: // multiply
          Log("--- Multiply ---");
          if (ip + 4 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          Debug.Assert(modes[2] == 0, $"For a valid multiplication call, modes[2] must be zero. Value:'{modes[2]}'");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Log($"val1: {val1}");
          Log($"Mode: {modes[1]}, Program[ip + 2]: {program[ip + 2]} ");
          val2 = modes[1] == 0 ? program[program[ip + 2]] : program[ip + 2];
          Log($"val2: {val2}");
          Log($"Program[ip + 3]: {program[ip + 3]}, Multiply:{val1 * val2} ");
          Log($"Setting program[{program[ip + 3]}] to {val1 * val2}");
          program[program[ip + 3]] = val1 * val2;
          ip += 4;
          Log($"New ip:{ip}");
          break;

        case 3: // input
          Log("--- Input ---");
          if (ip + 2 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          Debug.Assert(modes[0] == 0, $"For a valid input call, modes[0] must be zero. Value:'{modes[0]}'");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          Log($"Setting program[{program[ip + 1]}] to {input}");
          program[program[ip + 1]] = input;
          ip += 2;
          Log($"New ip:{ip}");
          break;

        case 4: // output
          Log("--- Output ---");
          if (ip + 2 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Console.WriteLine($"Program output:{val1}");
          DiagnosticCode = val1;

          ip += 2;
          Log($"New ip:{ip}");
          break;

        case 5: // jump if true
          Log("--- Jump if true ---");
          if (ip + 3 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Log($"val1: {val1}");
          if (val1 > 0)
          {
            Log($"Mode: {modes[1]}, Program[ip + 2]: {program[ip + 2]} ");
            val2 = modes[1] == 0 ? program[program[ip + 2]] : program[ip + 2];
            Log($"Setting ip to val2:{val2}");
            ip = (int)val2;
          }
          else
            ip += 3;
          Log($"New ip:{ip}");
          break;

        case 6: // jump if false
          Log("--- Jump if false ---");
          if (ip + 3 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Log($"val1: {val1}");
          if (val1 == 0)
          {
            Log($"Mode: {modes[1]}, Program[ip + 2]: {program[ip + 2]} ");
            val2 = modes[1] == 0 ? program[program[ip + 2]] : program[ip + 2];
            Log($"Setting ip to val2:{val2}");
            ip = (int)val2;
          }
          else
            ip += 3;
          Log($"New ip:{ip}");
          break;

        case 7: // less than
          Log("--- Less Than ---");
          if (ip + 4 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          Debug.Assert(modes[2] == 0, $"For a valid less than call, modes[2] must be zero. Value:'{modes[2]}'");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Log($"val1: {val1}");
          Log($"Mode: {modes[1]}, Program[ip + 2]: {program[ip + 2]} ");
          val2 = modes[1] == 0 ? program[program[ip + 2]] : program[ip + 2];
          Log($"val2: {val2}");
          Log($"Program[ip + 3]: {program[ip + 3]}, LessThan:{val1 < val2} ");
          Log($"Setting program[{program[ip + 3]}] to {val1 < val2}");
          program[program[ip + 3]] = val1 < val2 ? 1 : 0;
          ip += 4;
          Log($"New ip:{ip}");
          break;

        case 8: // equal
          Log("--- Equals ---");
          if (ip + 4 >= program.Length)
            throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
          Debug.Assert(modes[2] == 0, $"For a valid equality call, modes[2] must be zero. Value:'{modes[2]}'");

          Log($"Mode: {modes[0]}, Program[ip + 1]: {program[ip + 1]} ");
          val1 = modes[0] == 0 ? program[program[ip + 1]] : program[ip + 1];
          Log($"val1: {val1}");
          Log($"Mode: {modes[1]}, Program[ip + 2]: {program[ip + 2]} ");
          val2 = modes[1] == 0 ? program[program[ip + 2]] : program[ip + 2];
          Log($"val2: {val2}");
          Log($"Program[ip + 3]: {program[ip + 3]}, Equal:{val1 == val2} ");
          Log($"Setting program[{program[ip + 3]}] to {val1 == val2}");
          program[program[ip + 3]] = val1 == val2 ? 1 : 0;
          ip += 4;
          Log($"New ip:{ip}");
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }

  private static (int[] modes, int opcode) GetOpCode(int ip, long[] program)
  {
    int[] modes = new int[10];
    var instruction = (int)program[ip];
    var opcode = instruction % 100;
    instruction /= 100;
    int index = 0;
    while (instruction > 0)
    {
      modes[index++] = instruction % 10;
      instruction /= 10;
    }

    return (modes, opcode);
  }

  private static void Log(string message)
  {
    if (_verbose)
    {
      Console.WriteLine(message);
    }
  }
}
