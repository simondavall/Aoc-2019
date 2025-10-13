using System.Diagnostics;

namespace ShipComputer;

public class IntcodeProgram
{
  // Set _verbose to true to activate the console log oputput diagnostics
  private static readonly bool _verbose = false;
  private static long[] _ram = [];
  private static long _ip;
  public static long DiagnosticCode { get; private set; }

  public static void Execute(long[] rom, int input = 0)
  {
    _ip = 0;
    _ram = new long[rom.Length];
    Array.Copy(rom, _ram, rom.Length);

    while (_ip < _ram.Length && _ram[_ip] != 99)
    {
      var (modes, opCode) = GetNextOpCode();
      switch (opCode)
      {
        case 1: // add
          Actions.Add(modes);
          break;

        case 2: // multiply
          Actions.Multiply(modes);
          break;

        case 3: // input
          Actions.Input(modes, input);
          break;

        case 4: // output
          Actions.Output(modes);
          break;

        case 5: // jump if true
          Actions.JumpIfTrue(modes);
          break;

        case 6: // jump if false
          Actions.JumpIfFalse(modes);
          break;

        case 7: // less than
          Actions.LessThan(modes);
          break;

        case 8: // equal
          Actions.Equals(modes);
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }

  private static class Actions
  {
    private static long a = 0, b = 0, w = 0;
    internal static void Add(int[] modes)
    {
      Log("--- Add ---");
      if (_ip + 4 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
      Debug.Assert(modes[2] == 0, $"For a valid addition call, modes[2] must be zero. Value:'{modes[2]}'");

      _ip++;
      var addr = _ram[_ip++];
      a = modes[0] == 0 ? _ram[addr] : addr;
      addr = _ram[_ip++];
      b = modes[1] == 0 ? _ram[addr] : addr;
      w = _ram[_ip++];
      Log($"Setting Ram[{w}] to {a + b}");
      _ram[w] = a + b;
      Log($"New ip:{_ip}");
    }

    internal static void Multiply(int[] modes)
    {
      Log("--- Multiply ---");
      if (_ip + 4 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
      Debug.Assert(modes[2] == 0, $"For a valid multiplication call, modes[2] must be zero. Value:'{modes[2]}'");

      _ip++;
      var addr = _ram[_ip++];
      a = modes[0] == 0 ? _ram[addr] : addr;
      addr = _ram[_ip++];
      b = modes[1] == 0 ? _ram[addr] : addr;
      w = _ram[_ip++];
      Log($"Setting Ram[{w}] to {a * b}");
      _ram[w] = a * b;
      Log($"New ip:{_ip}");
    }

    internal static void Input(int[] modes, int input)
    {
      Log("--- Input ---");
      if (_ip + 2 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
      Debug.Assert(modes[0] == 0, $"For a valid input call, modes[0] must be zero. Value:'{modes[0]}'");

      _ip++;
      var addr = _ram[_ip++];
      _ram[addr] = input;
      Log($"New ip:{_ip}");
    }

    internal static void Output(int[] modes)
    {
      Log("--- Output ---");
      if (_ip + 2 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

      _ip++;
      var addr = _ram[_ip++];
      a = modes[0] == 0 ? _ram[addr] : addr;
      Console.WriteLine($"Program output:{a}");
      DiagnosticCode = a;
      Log($"New ip:{_ip}");
    }

    internal static void JumpIfTrue(int[] modes)
    {
      Log("--- Jump if true ---");
      if (_ip + 3 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

      _ip++;
      var addr = _ram[_ip++];
      a = modes[0] == 0 ? _ram[addr] : addr;
      if (a > 0)
      {
        addr = _ram[_ip++];
        b = modes[1] == 0 ? _ram[addr] : addr;
        Log($"Setting ip to b:{b}");
        _ip = (int)b;
      }
      else
        _ip++;
      Log($"New ip:{_ip}");
    }

    internal static void JumpIfFalse(int[] modes)
    {
      Log("--- Jump if false ---");
      if (_ip + 3 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

      _ip++;
      var addr = _ram[_ip++];
      a = modes[0] == 0 ? _ram[addr] : addr;
      if (a == 0)
      {
        addr = _ram[_ip++];
        b = modes[1] == 0 ? _ram[addr] : addr;
        Log($"Setting ip to b:{b}");
        _ip = (int)b;
      }
      else
        _ip++;
      Log($"New ip:{_ip}");
    }

    internal static void LessThan(int[] modes)
    {
      Log("--- Less Than ---");
      if (_ip + 4 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
      Debug.Assert(modes[2] == 0, $"For a valid less than call, modes[2] must be zero. Value:'{modes[2]}'");

      _ip++;
      var addr = _ram[_ip++];
      a = modes[0] == 0 ? _ram[addr] : addr;
      addr = _ram[_ip++];
      b = modes[1] == 0 ? _ram[addr] : addr;
      w = _ram[_ip++];
      Log($"Setting Ram[{w}] to {a < b}");
      _ram[w] = a < b ? 1 : 0;
      Log($"New ip:{_ip}");
    }

    internal static void Equals(int[] modes)
    {
      Log("--- Equals ---");
      if (_ip + 4 >= _ram.Length)
        throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
      Debug.Assert(modes[2] == 0, $"For a valid equality call, modes[2] must be zero. Value:'{modes[2]}'");

      _ip++;
      var addr = _ram[_ip++];

      a = modes[0] == 0 ? _ram[addr] : addr;
      addr = _ram[_ip++];

      b = modes[1] == 0 ? _ram[addr] : addr;
      w = _ram[_ip++];

      Log($"Setting Ram[{w}] to {a == b}");
      _ram[w] = a == b ? 1 : 0;
      Log($"New ip:{_ip}");
    }
  }

  private static (int[] modes, int opcode) GetNextOpCode()
  {
    int[] modes = new int[10];
    var instruction = (int)_ram[_ip];
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
