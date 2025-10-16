using System.Diagnostics;

namespace Spacecraft;

public class IntcodeComputer
{
  private long _ip;
  private long _offset;
  private const int MAX_RAM = 10_000;
  private readonly long[] _ram = [];
  private bool _isHalted = false;
  private bool _isAwaitingInput = false;
  private readonly List<long> _output = [];
  private readonly long[] param = new long[3];
  private int[] _paramMode = [];

  public IntcodeComputer(long[] program)
  {
    Debug.Assert(program.Length < MAX_RAM, $"Out Of Memory. Program is too large, get some more RAM. Size:{program.Length}");
    _ip = 0;
    _offset = 0;
    _ram = new long[MAX_RAM];
    Array.Copy(program, _ram, program.Length);
  }

  public void Execute()
  {
    while (!_isAwaitingInput && !_isHalted)
    {
      Debug.Assert(_ip < _ram.Length && _ip >= 0, $"Instruction pointer is out of bounds. Terminaling program. Ip:{_ip}");

      var opCode = GetNextOpCode();
      switch (opCode)
      {
        case 1:
          Add();
          break;

        case 2:
          Multiply();
          break;

        case 3:
          _isAwaitingInput = true;
          break;

        case 4:
          _output.Add(Output());
          break;

        case 5:
          JumpIfTrue();
          break;

        case 6:
          JumpIfFalse();
          break;

        case 7:
          LessThan();
          break;

        case 8:
          Equals();
          break;

        case 9:
          AdjustRelativeBaseOffset();
          break;

        case 99:
          _isHalted = true;
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }

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
      Input(value);
      _isAwaitingInput = false;
    }
  }

  public long[] FullOutput => _output.ToArray();

  public long GetLastOutput => _output.Count > 0 ? _output.Last() : 0;

  public bool IsHalted => _isHalted;

  public bool IsAwaitingInput => _isAwaitingInput;

  private int GetNextOpCode()
  {
    int[] modes = new int[3];
    var instruction = (int)_ram[_ip];
    var opcode = instruction % 100;
    instruction /= 100;
    int index = 0;
    while (instruction > 0)
    {
      modes[index++] = instruction % 10;
      instruction /= 10;
    }

    _paramMode = modes;

    return opcode;
  }

  private void Add()
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    param[1] = GetParam(_paramMode[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] + param[1];
    _ip += 4;
  }

  private void Multiply()
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    param[1] = GetParam(_paramMode[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] * param[1];
    _ip += 4;
  }

  private void Input(long input)
  {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = _paramMode[0] switch {
      0 => _ram[_ip + 1],
      2 => _ram[_ip + 1] + _offset,
      _ => throw new ApplicationException($"Invalid parameter mode for Input: {_paramMode[0]}")
    };

    _ram[param[0]] = input;
    _ip += 2;
  }

  private long Output()
  {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    _ip += 2;

    return param[0];
  }

  private void JumpIfTrue()
  {
    Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    param[1] = GetParam(_paramMode[1], _ip + 2);
    if (param[0] > 0)
      _ip = param[1];
    else
      _ip += 3;
  }

  private void JumpIfFalse()
  {
    Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    param[1] = GetParam(_paramMode[1], _ip + 2);
    if (param[0] == 0)
      _ip = param[1];
    else
      _ip += 3;
  }

  private void LessThan()
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    param[1] = GetParam(_paramMode[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] < param[1] ? 1 : 0;
    _ip += 4;
  }

  private void Equals()
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    param[1] = GetParam(_paramMode[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] == param[1] ? 1 : 0;
    _ip += 4;
  }

  private void AdjustRelativeBaseOffset()
  {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(_paramMode[0], _ip + 1);
    _offset += param[0];
    _ip += 2;
  }

  private long GetParam(int mode, long ip)
  {
    return mode switch
    {
      0 => _ram[_ram[ip]],
      1 => _ram[ip],
      2 => _ram[_ram[ip] + _offset],
      _ => throw new ApplicationException($"Unknown parameter mode. Value:'{mode}'")
    };
  }


}
