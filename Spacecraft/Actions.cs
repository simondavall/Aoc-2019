using System.Diagnostics;

internal class Actions
{
  private long _ip;
  private readonly long[] _ram;
  private readonly long[] param = new long[3];

  internal Actions(long[] ram)
  {
    _ip = 0;
    _ram = ram;
  }

  public long CurrentIp => _ip;

  internal void Add(int[] modes)
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    param[1] = GetParam(modes[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] + param[1];
    _ip += 4;
  }

  internal void Multiply(int[] modes)
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    param[1] = GetParam(modes[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] * param[1];
    _ip += 4;
  }

  internal void Input(long input)
  {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = _ram[_ip + 1];
    _ram[param[0]] = input;
    _ip += 2;
  }

  internal long Output(int[] modes)
  {
    Debug.Assert(_ip + 2 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    _ip += 2;

    return param[0];
  }

  internal void JumpIfTrue(int[] modes)
  {
    Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    param[1] = GetParam(modes[1], _ip + 2);
    if (param[0] > 0)
      _ip = param[1];
    else
      _ip += 3;
  }

  internal void JumpIfFalse(int[] modes)
  {
    Debug.Assert(_ip + 3 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    param[1] = GetParam(modes[1], _ip + 2);
    if (param[0] == 0)
      _ip = param[1];
    else
      _ip += 3;
  }

  internal void LessThan(int[] modes)
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    param[1] = GetParam(modes[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] < param[1] ? 1 : 0;
    _ip += 4;
  }

  internal void Equals(int[] modes)
  {
    Debug.Assert(_ip + 4 < _ram.Length, "Out of memory error. Instruction pointer requires more RAM to complete task.");

    param[0] = GetParam(modes[0], _ip + 1);
    param[1] = GetParam(modes[1], _ip + 2);
    param[2] = _ram[_ip + 3];
    _ram[param[2]] = param[0] == param[1] ? 1 : 0;
    _ip += 4;
  }

  private long GetParam(int mode, long ip)
  {
    return mode switch
    {
      0 => _ram[_ram[ip]],
      1 => _ram[ip],
      _ => throw new ApplicationException($"Unknown parameter mode. Value:'{mode}'")
    };
  }
}
