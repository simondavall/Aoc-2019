using Spacecraft;

namespace Day15;

internal static partial class Program {
  private static (int dx, int dy)[] Directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];

  private static long PartOne(long[] program) {
    var computer = new IntcodeComputer(program);
    while(!computer.IsAwaitingInput){
      computer.Execute();
    }

    var start = ((0, 0), computer);

    var seen = new HashSet<(int, int)>();
    var q = new PriorityQueue<((int x, int y), IntcodeComputer), int>();
    q.Enqueue(start, 0);
    seen.Add((0,0));

    while (q.Count > 0) {
      if (!q.TryDequeue(out var position, out int depth)) throw new ApplicationException("Queue was empty");
      (var cur, IntcodeComputer ic) = position;
      if(ic.IsHalted || !ic.IsAwaitingInput) 
        throw new ApplicationException("Computer in unexpected state");
      
      for (var dir = 0; dir < Directions.Length; dir++){
        var (dx, dy) = Directions[dir]; 
        var next = (cur.x + dx, cur.y + dy);
        if (seen.Add(next)) {
          var nComputer = ic.Clone();
          nComputer.SetInput(dir + 1);
          while(!nComputer.IsHalted && !nComputer.IsAwaitingInput){
            nComputer.Execute();
          }
          var output = nComputer.GetOutput();
          if (output.Length == 0)
            throw new ApplicationException("Expected computer output");
          if (output[0] == 0){
            continue; // hit a wall, cannot continue
          }
          else if (output[0] == 1){
            q.Enqueue((next, nComputer), depth + 1); // can proceed, add point to queue with increased depth
          }
          else if (output[0] == 2){
            return depth + 1; // found oxygen tank, so display path
          }
        }
      }
    }

    return -1;
  }

  private static long PartTwo(long[] program) {
    var computer = new IntcodeComputer(program);
    while(!computer.IsAwaitingInput){
      computer.Execute();
    }

    var start = ((0, 0), computer);
    var maxDepth = 0;

    var seen = new HashSet<(int, int)>();
    var q = new PriorityQueue<((int x, int y), IntcodeComputer), int>();
    q.Enqueue(start, 0);
    seen.Add((0,0));

    while (q.Count > 0) {
      if (!q.TryDequeue(out var position, out int depth)) throw new ApplicationException("Queue was empty");
      maxDepth = Math.Max(maxDepth, depth);
      (var cur, IntcodeComputer ic) = position;
      if(ic.IsHalted || !ic.IsAwaitingInput) 
        throw new ApplicationException("Computer in unexpected state");
      
      for (var dir = 0; dir < Directions.Length; dir++){
        var (dx, dy) = Directions[dir]; 
        var next = (cur.x + dx, cur.y + dy);
        if (seen.Add(next)) {
          var nComputer = ic.Clone();
          nComputer.SetInput(dir + 1);
          while(!nComputer.IsHalted && !nComputer.IsAwaitingInput){
            nComputer.Execute();
          }
          var output = nComputer.GetOutput();
          if (output.Length == 0)
            throw new ApplicationException("Expected computer output");
          if (output[0] == 0){
            continue; // hit a wall, cannot continue
          }
          else if (output[0] == 1){
            q.Enqueue((next, nComputer), depth + 1); // can proceed, add point to queue with increased depth
          }
          else if (output[0] == 2){
            q.Clear();
            q.Enqueue((next, nComputer), 0);
            seen.Clear();
            seen.Add(next);
            
            break; 
          }
        }
      }
    }

    return maxDepth;
  }
}
