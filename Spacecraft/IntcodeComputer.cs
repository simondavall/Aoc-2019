using System.Diagnostics;

namespace Spacecraft;

public class IntcodeComputer
{
  private readonly long[] _ram = [];
  private bool _isHalted = false;
  private bool _isAwaitingInput = false;
  private readonly Stack<long> _output = [];
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

  public long GetLastOutput => _output.Count > 0 ? _output.Peek() : 0;

  public bool IsHalted => _isHalted;

  public bool IsAwaitingInput => _isAwaitingInput;

  public long Ip => _actions.CurrentIp;

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
