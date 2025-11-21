using AocHelper;

namespace Day14;

internal static partial class Program {
  private static long PartOne(Reaction[] reactions) {
    var excess = new Dictionary<string, long>();
    var finalAmount = new Dictionary<string, long>();

    foreach (var r in reactions)
      excess[r.Output.Name] = 0;

    List<Chemical> result = [];
    var fuel = reactions.SingleOrDefault(x => x.Output.Name == "FUEL")
      ?? throw new ApplicationException("Did not find reaction for FUEL");

    ConsoleWriteLine($"Add {fuel.Inputs.Print()} to queue");
    var q = new Queue<Chemical>(fuel.Inputs);
    while (q.Count > 0) {
      var cur = q.Dequeue();
      ConsoleWriteLine($"Working with {cur}");
      if (excess[cur.Name] > cur.Amount) {
        excess[cur.Name] -= cur.Amount;
        cur.Amount = 0;
      } else {
        cur.Amount -= excess[cur.Name];
        excess[cur.Name] = 0;
      }
      ConsoleWriteLine($"After removing excess: {cur}");
      var reaction = reactions.SingleOrDefault(x => x.Output.Name == cur.Name)
        ?? throw new ApplicationException($"Did not find reaction for {cur.Name}");

      if (reaction.Inputs[0].Name == "ORE") {
        ConsoleWriteLine($"Found ORE requirer, adding to final Amount: {cur}");
        finalAmount.AddOrUpdate(cur.Name, cur.Amount);
        ConsoleWriteLine($"finalAmount: {finalAmount.Print()}");
        continue;
      }

      var requiredAmount = cur.Amount;
      ConsoleWriteLine($"Required amount: {requiredAmount} of {reaction.Output}");
      var createMultiplier = DivideAndRoundUp(requiredAmount, reaction.Output.Amount);
      ConsoleWriteLine($"Create Multiplier: {createMultiplier}");
      var amountCreated = reaction.Output.Amount * createMultiplier;
      ConsoleWriteLine($"Amount created: {amountCreated}");
      excess[reaction.Output.Name] += amountCreated - cur.Amount;

      foreach (var chem in reaction.Inputs) {
        ConsoleWriteLine($"Adding {chem * createMultiplier} to the queue");
        q.Enqueue(chem * createMultiplier);
      }
      ConsoleWriteLine($"Queue contents: {q.ToArray().Print()}");
    }

    long tally = 0;
    foreach (var (k, v) in finalAmount) {
      var chem = reactions.Single(x => x.Output.Name == k);
      ConsoleWriteLine($"Chemical:{chem}, Amount:{v}");
      var xx = DivideAndRoundUp(v, chem.Output.Amount);
      ConsoleWriteLine($"Multiplier: {xx}");
      var ore = chem.Inputs[0].Amount * xx;
      tally += ore;
    }

    return tally;
  }

  private static long PartTwo(Reaction[] reactions) {
    var fuelMultiplier = 100;
    foreach(var r in reactions){
      r.Output.Amount *= fuelMultiplier;
      Array.ForEach(r.Inputs, x => x.Amount *= fuelMultiplier);
    }
    
    var oreRequired = PartOne(reactions);

    return oreRequired;
  }

  private static void AddOrUpdate(this Dictionary<string, long> dict, string key, long value) {
    dict.TryGetValue(key, out long existing);
    dict[key] = existing + value;
  }

  private static long DivideAndRoundUp(long a, long b) {
    return (long)Math.Ceiling((double)a / b);
  }

  private class Reaction(Chemical[] chemicals) {
    public Chemical[] Inputs { get; set; } = chemicals[..^1];
    public Chemical Output { get; set; } = chemicals[^1];

    public override string ToString() {
      return $"{Inputs.Print()} => {Output}";
    }
  }

  private class Chemical(string name, long amount) {
    public string Name { get; set; } = name;
    public long Amount { get; set; } = amount;

    public override string ToString() {
      return $"{Name} [{Amount}]";
    }

    public static Chemical operator *(Chemical a, long b) {
      var chem = new Chemical(a.Name, a.Amount * b);
      return chem;
    }
  }
}
