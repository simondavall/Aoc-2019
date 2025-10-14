using System.Diagnostics;

namespace Spacecraft;

public class IntcodeComputer
{
  private readonly bool _isLoggingActive = false;
  private readonly long[] _rom = [];
  private readonly long[] _ram = [];
  private long _ip;
  private readonly Stack<long> _output = [];

  public IntcodeComputer(long[] program)
  {
    _rom = program; 
    _ram = new long[program.Length];
    Array.Copy(program, _ram, program.Length);
  }

  internal static Dictionary<(int phase, long input), long> _cache = [];

  public static long Acs(Queue<int> phasing, long[] program)
  {
    _cache = [];
    var input = 0L;
    var output = 0L;
    while (true)
    {
      if (!phasing.TryDequeue(out var phase))
        break;

      if (_cache.TryGetValue((phase, input), out var cachedItem)){
        output = cachedItem;
      }
      else{
        var core = new IntcodeComputer(program);
        core.Execute(new Queue<long>([phase, input]));
        output = core.GetOutput().FirstOrDefault();
        _cache[(phase, input)] = output;
      }

      input = output;
    }

    return output;
  }

  // public static long AcsFeedback(int[] phasing, int[] feedbackPhasing, long[] program)
  // {
  //   _cache = [];
  //   var input = 0L;
  //   var output = 0L;
  //
  //   List<IntcodeComputer> amps = [];
  //   for (int i = 0; i < 5; i++){
  //     amps.Add(new IntcodeComputer(program));
  //   }
  //
  //   while (true)
  //   {
  //     for(var i = 0; i < 5; i++){
  //       if (!amps[i].IsHalted){
  //         amps[i].Execute(new Queue<long>([phasing[i], input]));
  //
  //       }
  //     }
  //
  //     if (_cache.TryGetValue((phase, input), out var cachedItem)){
  //       output = cachedItem;
  //     }
  //     else{
  //       var core = new IntcodeComputer(program);
  //       core.Execute(new Queue<long>([phase, input]));
  //       output = core.GetOutput();
  //       _cache[(phase, input)] = output;
  //     }
  //
  //     input = output;
  //   }
  //
  //   return output;
  // }

  public long[] GetOutput()
  {
    return _output.ToArray();
  }

  public long ReadMemory(long address){
    if (address < 0 || address >= _ram.Length)
      return _ram.FirstOrDefault();

    return _ram[address];  
  }

  public void SetMemory(long address, long value){
    if (address >= 0 && address < _ram.Length)
      _ram[address] = value;
  }

  public void Reset(){
    Array.Copy(_rom, _ram, _rom.Length);
    _cache = [];
    _ip = 0;
  }

  public void Execute(Queue<long> inputs)
  {
    bool isHalted = false;
    _ip = 0;

    while (_ip < _ram.Length && !isHalted)
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

        case 4:;
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
          isHalted = true;
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }

  private long a = 0, b = 0, w = 0;
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
    _output.Push(a);
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
