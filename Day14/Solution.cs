using AocHelper;

namespace Day14;

internal static partial class Program {

  private static long PartOne(Reaction[] reactions) {
    Dictionary<string, long> _excess = [];
    var finalAmount = new Dictionary<string, long>();

    foreach (var r in reactions)
      _excess[r.Output.Name] = 0;

    List<Chemical> result = [];
    var fuel = reactions.Single(x => x.Output.Name == "FUEL");

    var q = new Queue<Chemical>(fuel.Inputs);
    while (q.Count > 0) {
      var cur = q.Dequeue();
      if (_excess[cur.Name] > cur.Amount) {
        _excess[cur.Name] -= cur.Amount;
        cur.Amount = 0;
      } else {
        cur.Amount -= _excess[cur.Name];
        _excess[cur.Name] = 0;
      }
      var reaction = reactions.Single(x => x.Output.Name == cur.Name);

      if (reaction.Inputs[0].Name == "ORE") {
        finalAmount.AddOrUpdate(cur.Name, cur.Amount);
        continue;
      }

      var requiredAmount = cur.Amount;
      var createMultiplier = DivideAndRoundUp(requiredAmount, reaction.Output.Amount);
      var amountCreated = reaction.Output.Amount * createMultiplier;
      _excess[reaction.Output.Name] += amountCreated - cur.Amount;

      foreach (var chem in reaction.Inputs) {
        q.Enqueue(chem * createMultiplier);
      }
    }

    long totalOreRequired = 0;
    foreach (var (k, v) in finalAmount) {
      var chem = reactions.Single(x => x.Output.Name == k);
      var multiplier = DivideAndRoundUp(v, chem.Output.Amount);
      var ore = chem.Inputs[0].Amount * multiplier;
      totalOreRequired += ore;
    }
    return totalOreRequired;
  }

  private static long PartTwo(Reaction[] reactions) {
    const long OneTrillion = 1_000_000_000_000;
    long fuelAmount = 1_000_000;

    var origFuel = reactions.Single(x => x.Output.Name == "FUEL");
    var origFuelInputs = new List<Chemical>();
    Array.ForEach(origFuel.Inputs, x => origFuelInputs.Add(new Chemical(x.Name, x.Amount)));

    long increment = 1<<20; // 2 pow 20 = 1048576
    var tooHigh = false;
    while (true) {
      var fuel = reactions.Single(x => x.Output.Name == "FUEL");
      for(var i = 0; i < fuel.Inputs.Length; i++)
        fuel.Inputs[i].Amount = origFuelInputs[i].Amount * fuelAmount;

      var oreRequired = PartOne(reactions);
      if (oreRequired > OneTrillion){
        if (!tooHigh){
          tooHigh = true;
          increment = ReduceIncrement(increment);
        } 
        fuelAmount -= increment;
      } else if (oreRequired < OneTrillion) {
        if (increment == 1){
          return fuelAmount;
        }
        if (tooHigh){
          tooHigh = false;
          increment = ReduceIncrement(increment);
        }
        fuelAmount += increment;
      }
    }
  }

  private static long ReduceIncrement(long increment){
    increment /= 2;
    return increment > 0 ? increment : 1;
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
