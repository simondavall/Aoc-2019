using System.Diagnostics;

namespace Spacecraft;

public class IntcodeComputer
{
  private readonly bool _isLoggingActive;
  private readonly long[] _rom = [];
  private readonly long[] _ram = [];
  private long _ip;
  private bool _isHalted = false;
  private readonly Queue<long> _input = [];
  private readonly Queue<long> _output = [];
  private long _lastOutput;
  private long a = 0, b = 0, w = 0;

  public IntcodeComputer(long[] program)
  {
    _isLoggingActive = false;
    _rom = program;
    _ram = new long[program.Length];
    Array.Copy(program, _ram, program.Length);
  }

  public static long AcsWithFeedback(Queue<int> phasing, long[] program)
  {
    long input = 0;
    long output = 0;
    IntcodeComputer[] amps = new IntcodeComputer[5];

    int currentAmp = 0;

    while (true)
    {
      if (!phasing.TryDequeue(out var phase))
        break;

      var q = new Queue<long>([phase, input]);

      amps[currentAmp] = new IntcodeComputer(program);
      amps[currentAmp].Execute(q);
      output = amps[currentAmp].GetLastOutput;

      input = output;
      currentAmp++;
    }

    return output;
  }

  public static long Acs(Queue<int> phasing, long[] program)
  {
    long input = 0;
    long output = 0;


    while (true)
    {
      if (!phasing.TryDequeue(out var phase))
        break;

      var q = new Queue<long>([phase, input]);

      var amp = new IntcodeComputer(program);
      amp.Execute(q);
      output = amp.GetLastOutput;

      input = output;
    }

    return output;
  }

  public void Execute(Queue<long> inputs)
  {
    _ip = 0;

    while (_ip < _ram.Length && !_isHalted)
    {
      var (modes, opCode) = GetNextOpCode();
      switch (opCode)
      {
        case 1:
          Add(modes);
          break;

        case 2:
          Multiply(modes);
          break;

        case 3:
          if (!inputs.TryDequeue(out var input))
            throw new ApplicationException("Expected an input, none found.");
          Input(modes, input);
          break;

        case 4:
          ;
          Output(modes);
          break;

        case 5:
          JumpIfTrue(modes);
          break;

        case 6:
          JumpIfFalse(modes);
          break;

        case 7:
          LessThan(modes);
          break;

        case 8:
          Equals(modes);
          break;

        case 99:
          _isHalted = true;
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }

  public long[] GetOutput()
  {
    return _output.ToArray();
  }

  public long GetLastOutput => _lastOutput;

  public long ReadMemory(long address)
  {
    if (address < 0 || address >= _ram.Length)
      return _ram.FirstOrDefault();

    return _ram[address];
  }

  public void SetMemory(long address, long value)
  {
    if (address >= 0 && address < _ram.Length)
      _ram[address] = value;
  }

  public void Reset()
  {
    Array.Copy(_rom, _ram, _rom.Length);
    //_cache = [];
    _ip = 0;
  }

  public bool IsHalted => _isHalted;

  internal void Add(int[] modes)
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

  internal void Multiply(int[] modes)
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

  internal void Input(int[] modes, long input)
  {
    Log("--- Input ---");
    if (_ip + 2 >= _ram.Length)
      throw new IndexOutOfRangeException("Instruction pointer beyond end of program");
    Debug.Assert(modes[0] == 0, $"For a valid input call, modes[0] must be zero. Value:'{modes[0]}'");

    _ip++;
    var addr = _ram[_ip++];

    //Console.WriteLine($"Saving input {input} to memory slot {addr}");
    _ram[addr] = input;
    Log($"New ip:{_ip}");
  }

  internal void Output(int[] modes)
  {
    Log("--- Output ---");
    if (_ip + 2 >= _ram.Length)
      throw new IndexOutOfRangeException("Instruction pointer beyond end of program");

    _ip++;
    var addr = _ram[_ip++];
    a = modes[0] == 0 ? _ram[addr] : addr;
    _output.Enqueue(a);
    _lastOutput = a;

    //Console.WriteLine($"Saving output {a}");
    Log($"New ip:{_ip}");
  }

  internal void JumpIfTrue(int[] modes)
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

  internal void JumpIfFalse(int[] modes)
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

  internal void LessThan(int[] modes)
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

  internal void Equals(int[] modes)
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

  private (int[] modes, int opcode) GetNextOpCode()
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

  private void Log(string message)
  {
    if (_isLoggingActive)
    {
      Console.WriteLine(message);
    }
  }
}
