public static class GroupHelper
{
    public static void Print(Dictionary<string, double> keyValues, int startIndex, int groupLen,
         string keyAlias, string valueAlias, string valueSuffix, string aggregateAlias, string aggregateSuffix, Func<Dictionary<string, double>, double> aggregateFn)
    {
        var super_i = 0;
        var super_aggrs = new Dictionary<string, double>();

        for (int i = startIndex; i < keyValues.Count; i += groupLen)
        {
            var g_pairs = keyValues.Skip(i).Take(groupLen).ToDictionary(x => x.Key, x => x.Value);

            Console.WriteLine("[Group-" + (super_i / groupLen + 1).ToString("00") + "]\n");

            var sub_i = 0;
            foreach (var kvp in g_pairs)
            {
                sub_i++;
                super_aggrs.Add(kvp.Key, kvp.Value);
                Console.WriteLine($"{super_i + sub_i}.{keyAlias}: {kvp.Key} => {valueAlias}: {kvp.Value}{valueSuffix}");
            }
            super_i += sub_i;

            Console.WriteLine($"->{aggregateAlias}: {(aggregateFn(g_pairs))}{aggregateSuffix} (in {sub_i} {keyAlias}s)");
            Console.WriteLine();
            Console.WriteLine($"=> {aggregateAlias.ToUpper()}: {(aggregateFn(super_aggrs))}{aggregateSuffix.ToUpper()} (in {super_i} {keyAlias.ToUpper()}S)");
            Console.WriteLine();
            Console.WriteLine("_____________________________________\n");
        }
    }


    public static bool CheckAgg(Dictionary<string, double> keyValues, int startIndex, int groupLen,
         string keyAlias, string valueAlias, string valueSuffix, string aggregateAlias, string aggregateSuffix,
         Func<Dictionary<string, double>, double> aggregateFn, Func<double, bool> invalidityPred)
    {
        for (int i = startIndex; i < keyValues.Count; i += groupLen)
        {
            var g_pairs = keyValues.Skip(i).Take(groupLen).ToDictionary(x => x.Key, x => x.Value);

            if (g_pairs.Count() == groupLen)
            {
                bool isValid = !invalidityPred(aggregateFn(g_pairs));
                if (!isValid) return true;
            }
        }
        return false;
    }
}

public static class GroupCombinator
{
    public static void PrintCombinations(Dictionary<string, double> keyValues, string keyAlias,
        string valueAlias, string valueSuffix, string aggregateAlias, string aggregateSuffix,
        Func<Dictionary<string, double>, double> aggregateFn, string? startKey = null,
        int? minGroupLen = null, int? maxGroupLen = null, bool checkInvalidity = false,
        Func<double, bool>? invalidityPred = null)
    {
        if (minGroupLen > maxGroupLen)
        {
            System.Console.WriteLine("minGroupLen cannot be greater than maxGroupLen");
            return;
        }
        else if (maxGroupLen > keyValues.Count())
        {
            System.Console.WriteLine("invalid maxGroupLen");
            return;
        }
        else if (checkInvalidity && invalidityPred == null)
        {
            System.Console.WriteLine("you must define a invalidity predicate function");
            return;
        }

        var fnCombinate = (int startIndex, int groupLen) =>
        {
            Console.WriteLine();
            Console.WriteLine($"####### GROUP_SIZE: {groupLen}; START_{keyAlias.ToUpper()}: {keyValues.Keys.ElementAt(startIndex)} #######");
            Console.WriteLine();

            if (checkInvalidity && GroupHelper.CheckAgg(keyValues, startIndex, groupLen, keyAlias,
                    valueAlias, valueSuffix, aggregateAlias, aggregateSuffix, aggregateFn, invalidityPred!))
                System.Console.WriteLine("<skipped>!\n");
            else
                GroupHelper.Print(keyValues, startIndex, groupLen, keyAlias,
                    valueAlias, valueSuffix, aggregateAlias, aggregateSuffix, aggregateFn);
        };

        int start_index_pick = startKey != null ? keyValues.Keys.ToList().IndexOf(startKey) : -1;

        for (int groupLen = minGroupLen ?? keyValues.Count(); groupLen <= (maxGroupLen ?? keyValues.Count()); groupLen++)
        {
            if (start_index_pick != -1)
                fnCombinate(start_index_pick, groupLen);
            else
                for (int startIndex = 0; startIndex < groupLen; startIndex++)
                    fnCombinate(startIndex, groupLen);
        }
    }
}
