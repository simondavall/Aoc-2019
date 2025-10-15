using System.Diagnostics;

namespace Spacecraft;

public class IntcodeComputer
{
  private readonly long[] _ram = [];
  private bool _isHalted = false;
  private bool _isAwaitingInput = false;
  private readonly Stack<long> _output = [];
  private readonly long[] param = new long[3];
  private readonly Actions _actions;

  public IntcodeComputer(long[] program)
  {
    _ram = new long[program.Length];
    Array.Copy(program, _ram, program.Length);
    _actions = new Actions(_ram);
  }

  public void Execute()
  {
    while (!_isAwaitingInput && !_isHalted)
    {
      Debug.Assert(Ip < _ram.Length && Ip >= 0, $"Instruction pointer is out of bounds. Terminaling program. Ip:{Ip}");

      var (modes, opCode) = GetNextOpCode();
      switch (opCode)
      {
        case 1:
          _actions.Add(modes);
          break;

        case 2:
          _actions.Multiply(modes);
          break;

        case 3:
          _isAwaitingInput = true;
          break;

        case 4:
          _output.Push(_actions.Output(modes));
          break;

        case 5:
          _actions.JumpIfTrue(modes);
          break;

        case 6:
          _actions.JumpIfFalse(modes);
          break;

        case 7:
          _actions.LessThan(modes);
          break;

        case 8:
          _actions.Equals(modes);
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
    var output = new List<long>();
    while (_output.Count > 0)
    {
      output.Insert(0, _output.Pop());
    }
    return output.ToArray();
  }

  public long GetLastOutput => _output.Peek();

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

  public void SetInput(long value)
  {
    if (_isAwaitingInput)
    {
      _actions.Input(value);
      _isAwaitingInput = false;
    }
  }

  public bool IsHalted => _isHalted;

  public bool IsAwaitingInput => _isAwaitingInput;

  public long Ip => _actions.CurrentIp;


  // internal void Add(int[] modes)
  // {
  //   Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   param[1] = GetParam(_ip + 2, _ram, modes[1]);
  //   param[2] = _ram[_ip + 3];
  //   _ram[param[2]] = param[0] + param[1];
  //   _ip += 4;
  // }
  //
  // internal void Multiply(int[] modes)
  // {
  //   Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   param[1] = GetParam(_ip + 2, _ram, modes[1]);
  //   param[2] = _ram[_ip + 3];
  //   _ram[param[2]] = param[0] * param[1];
  //   _ip += 4;
  // }
  //
  // internal void Input(long input)
  // {
  //   Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = _ram[_ip + 1];
  //   _ram[param[0]] = input;
  //   _ip += 2;
  // }
  //
  // internal void Output(int[] modes)
  // {
  //   Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   _output.Push(param[0]);
  //   _ip += 2;
  // }
  //
  // internal void JumpIfTrue(int[] modes)
  // {
  //   Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   param[1] = GetParam(_ip + 2, _ram, modes[1]);
  //   if (param[0] > 0)
  //     _ip = param[1];
  //   else
  //     _ip += 3;
  // }
  //
  // internal void JumpIfFalse(int[] modes)
  // {
  //   Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   param[1] = GetParam(_ip + 2, _ram, modes[1]);
  //   if (param[0] == 0)
  //     _ip = param[1];
  //   else
  //     _ip += 3;
  // }
  //
  // internal void LessThan(int[] modes)
  // {
  //   Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   param[1] = GetParam(_ip + 2, _ram, modes[1]);
  //   param[2] = _ram[_ip + 3];
  //   _ram[param[2]] = param[0] < param[1] ? 1 : 0;
  //   _ip += 4;
  // }
  //
  // internal void Equals(int[] modes)
  // {
  //   Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
  //
  //   param[0] = GetParam(_ip + 1, _ram, modes[0]);
  //   param[1] = GetParam(_ip + 2, _ram, modes[1]);
  //   param[2] = _ram[_ip + 3];
  //   _ram[param[2]] = param[0] == param[1] ? 1 : 0;
  //   _ip += 4;
  // }
  //
  // private static long GetParam(long ip, long[] ram, int mode)
  // {
  //   return mode switch
  //   {
  //     0 => ram[ram[ip]],
  //     1 => ram[ip],
  //     _ => throw new ApplicationException($"Unknown parameter mode. Value:'{mode}'")
  //   };
  // }
  //
  private (int[] modes, int opcode) GetNextOpCode()
  {
    int[] modes = new int[10];
    var instruction = (int)_ram[Ip];
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
}
