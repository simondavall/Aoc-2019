using System.Diagnostics;

namespace Spacecraft;

public class IntcodeComputer {
  private long _ip;
  private long _relativeBaseOffset;
  private const int MAX_RAM = 10_000;
  private readonly long[] _ram = new long[MAX_RAM];
  private bool _isHalted = false;
  private bool _isAwaitingInput = false;
  private int _inputMode;
  private readonly List<long> _output = [];

  private IntcodeComputer(long[] ram, long ip, long relativeBaseOffset, bool isAwaitingInput, int inputMode) {
    Array.Copy(ram, _ram, ram.Length);
    _ip = ip;
    _relativeBaseOffset = relativeBaseOffset;
    _isAwaitingInput = isAwaitingInput;
    _inputMode = inputMode;
  }

  public IntcodeComputer(long[] program) {
    Debug.Assert(program.Length < MAX_RAM, $"Out Of Memory. Program is too large, get some more RAM. Size:{program.Length}");
    _ip = 0;
    _relativeBaseOffset = 0;
    Array.Copy(program, _ram, program.Length);
  }

  public IntcodeComputer Clone() {
    return new IntcodeComputer(_ram, _ip, _relativeBaseOffset, _isAwaitingInput, _inputMode);
  }

  public void Execute() {
    while (!_isAwaitingInput && !_isHalted) {
      Debug.Assert(_ip < _ram.Length && _ip >= 0, $"Instruction pointer is out of bounds. Terminaling program. Ip:{_ip}");

      var (opCode, modes) = GetNextOpCode();
      switch (opCode) {
        case 1:
          Add(modes);
          break;

        case 2:
          Multiply(modes);
          break;

        case 3:
          _isAwaitingInput = true;
          _inputMode = modes[0];
          break;

        case 4:
          _output.Add(Output(modes));
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

        case 9:
          AdjustRelativeBaseOffset(modes);
          break;

        case 99:
          _isHalted = true;
          break;

        default:
          throw new InvalidOperationException($"Unrecognized opCode: '{opCode}'");
      }
    }
  }

  public long ReadMemory(long address) {
    if (address < 0 || address >= _ram.Length)
      return _ram.FirstOrDefault();

    return _ram[address];
  }

  public void SetMemory(long address, long value) {
    if (address >= 0 && address < _ram.Length)
      _ram[address] = value;
  }

  public void SetInput(long value) {
    if (_isAwaitingInput) {
      Input(value);
      _isAwaitingInput = false;
    }
  }

  public long[] GetOutput() {
    long[] output = _output.ToArray();
    _output.Clear();
    return output;
  }

  public bool IsHalted => _isHalted;

  public bool IsAwaitingInput => _isAwaitingInput;

  private (int, int[]) GetNextOpCode() {
    int[] modes = new int[3];
    var instruction = (int)_ram[_ip];
    var opcode = instruction % 100;
    instruction /= 100;
    int index = 0;
    while (instruction > 0) {
      modes[index++] = instruction % 10;
      instruction /= 10;
    }

    return (opcode, modes);
  }

  private void Add(int[] modes) {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");
    long a = GetParam(modes[0], _ip + 1);
    long b = GetParam(modes[1], _ip + 2);
    long c = GetWriteParam(modes[2], _ip + 3);
    _ram[c] = a + b;
    _ip += 4;
  }

  private void Multiply(int[] modes) {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    long b = GetParam(modes[1], _ip + 2);
    long c = GetWriteParam(modes[2], _ip + 3);
    _ram[c] = a * b;
    _ip += 4;
  }

  private void Input(long input) {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetWriteParam(_inputMode, _ip + 1);
    _ram[a] = input;
    _ip += 2;
  }

  private long Output(int[] modes) {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    _ip += 2;

    return a;
  }

  private void JumpIfTrue(int[] modes) {
    Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    long b = GetParam(modes[1], _ip + 2);
    if (a > 0)
      _ip = b;
    else
      _ip += 3;
  }

  private void JumpIfFalse(int[] modes) {
    Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    long b = GetParam(modes[1], _ip + 2);
    if (a == 0)
      _ip = b;
    else
      _ip += 3;
  }

  private void LessThan(int[] modes) {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    long b = GetParam(modes[1], _ip + 2);
    long c = GetWriteParam(modes[2], _ip + 3);
    _ram[c] = a < b ? 1 : 0;
    _ip += 4;
  }

  private void Equals(int[] modes) {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    long b = GetParam(modes[1], _ip + 2);
    long c = GetWriteParam(modes[2], _ip + 3);
    _ram[c] = a == b ? 1 : 0;
    _ip += 4;
  }

  private void AdjustRelativeBaseOffset(int[] modes) {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    long a = GetParam(modes[0], _ip + 1);
    _relativeBaseOffset += a;
    _ip += 2;
  }

  private long GetParam(int mode, long ip) {
    return mode switch {
      0 => _ram[_ram[ip]],
      1 => _ram[ip],
      2 => _ram[_ram[ip] + _relativeBaseOffset],
      _ => throw new ApplicationException($"Unknown parameter mode. Value:'{mode}'")
    };
  }

  private long GetWriteParam(int mode, long ip) {
    return mode switch {
      0 => _ram[ip],
      1 => _ram[ip],
      2 => _ram[ip] + _relativeBaseOffset,
      _ => throw new ApplicationException($"Unknown parameter mode. Value:'{mode}'")
    };
  }
}
