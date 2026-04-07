using WeShareClone.Domain.Models;

namespace WeShareClone.Utilities;

/// <summary>
/// Calculates minimized debt payments for a settlement.
/// Rates are expressed as DKK per 100 units; DKK itself has an implicit rate of 100.
/// </summary>
public static class DebtCalculator
{
    private const decimal DkkRate = 100m;

    public record DebtPayment(int FromUserId, int ToUserId, decimal Amount);

    /// <summary>
    /// Calculates the minimum set of debt payments to settle all balances.
    /// Returns amounts in <paramref name="settlementCurrency"/>.
    /// </summary>
    public static DebtPayment[] Calculate(
        IEnumerable<Entry> entries,
        IEnumerable<int> participantIds,
        IEnumerable<ExchangeRate> exchangeRates,
        string settlementCurrency)
    {
        ExchangeRate[] rates = exchangeRates.ToArray();
        Dictionary<int, decimal> balances = participantIds.ToDictionary(id => id, _ => 0m);

        foreach (Entry entry in entries)
        {
            decimal entryValueInSettlement = ConvertCurrency(entry.Value, entry.Currency, settlementCurrency, rates);

            foreach (EntryDistribution dist in entry.Distributions)
            {
                decimal share = CalculateShare(entry, dist);
                decimal shareInSettlement = ConvertCurrency(share, entry.Currency, settlementCurrency, rates);

                if (!balances.ContainsKey(dist.UserId))
                    balances[dist.UserId] = 0;

                balances[dist.UserId] -= shareInSettlement;
            }

            if (balances.ContainsKey(entry.AddedBy))
                balances[entry.AddedBy] += entryValueInSettlement;
        }

        return MinimizeDebts(balances);
    }

    private static decimal CalculateShare(Entry entry, EntryDistribution dist)
        => entry.DistributionMode switch
        {
            // All factors are identical; share = value / participant count
            EDistributionMode.EvenSplit   => entry.Distributions.Count > 0
                ? entry.Value / entry.Distributions.Count
                : 0,

            // Factor represents the user's fractional share of the total
            EDistributionMode.Percentage  => entry.Value * dist.Factor,

            // Factor is the exact fixed amount owed
            EDistributionMode.FixedAmount => dist.Factor,

            _ => 0
        };

    /// <summary>
    /// Greedy debt minimization: repeatedly match the largest creditor with the largest debtor.
    /// </summary>
    private static DebtPayment[] MinimizeDebts(Dictionary<int, decimal> balances)
    {
        List<DebtPayment> payments = [];

        // Positive balance = creditor (others owe them); negative = debtor (they owe others)
        List<(int UserId, decimal Balance)> creditors = balances
            .Where(kv => kv.Value > 0.005m)
            .Select(kv => (kv.Key, kv.Value))
            .OrderByDescending(x => x.Value)
            .ToList();

        List<(int UserId, decimal Balance)> debtors = balances
            .Where(kv => kv.Value < -0.005m)
            .Select(kv => (kv.Key, kv.Value))
            .OrderBy(x => x.Value)
            .ToList();

        int ci = 0, di = 0;
        while (ci < creditors.Count && di < debtors.Count)
        {
            (int creditorId, decimal creditBalance) = creditors[ci];
            (int debtorId, decimal debtBalance)     = debtors[di];

            decimal amount = Math.Min(creditBalance, -debtBalance);
            amount = Math.Round(amount, 2);

            if (amount > 0.005m)
                payments.Add(new DebtPayment(debtorId, creditorId, amount));

            creditors[ci] = (creditorId, creditBalance - amount);
            debtors[di]   = (debtorId, debtBalance + amount);

            if (creditors[ci].Balance <= 0.005m) ci++;
            if (debtors[di].Balance   >= -0.005m) di++;
        }

        return [.. payments];
    }

    private static decimal ConvertCurrency(decimal value, string from, string to, ExchangeRate[] rates)
    {
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
            return value;

        decimal fromRate = GetRate(from, rates);
        decimal toRate   = GetRate(to, rates);

        if (fromRate == 0 || toRate == 0)
            return value;

        return value * fromRate / toRate;
    }

    private static decimal GetRate(string currency, ExchangeRate[] rates)
    {
        if (string.Equals(currency, "DKK", StringComparison.OrdinalIgnoreCase))
            return DkkRate;

        ExchangeRate? match = rates.FirstOrDefault(
            r => string.Equals(r.Currency, currency, StringComparison.OrdinalIgnoreCase)
        );

        return match?.Rate ?? 0m;
    }
}
