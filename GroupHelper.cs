public static class GroupHelper
{
    public static void Print(Dictionary<int, double> keyValues, int startIndex, int groupLen,
         string keyAlias, string valueAlias, string valueSuffix, string aggregateAlias, string aggregateSuffix, Func<Dictionary<int, double>, double> aggregateFn)
    {
        var super_i = 0;
        var super_aggrs = new Dictionary<int, double>();

        for (int i = startIndex; i < keyValues.Count; i += groupLen)
        {
            Console.WriteLine("_______________________________________");
            Console.WriteLine();

            var g_pairs = keyValues.Skip(i).Take(groupLen).ToDictionary(x => x.Key, x => x.Value);

            Console.WriteLine("[group-" + (super_i / groupLen + 1).ToString("00") + "]");

            var sub_i = 0;
            foreach (var kvp in g_pairs)
            {
                sub_i++;
                super_aggrs.Add(kvp.Key, kvp.Value);
                Console.WriteLine($"{super_i + sub_i}.{keyAlias}: {kvp.Key} => {valueAlias}: {kvp.Value}{valueSuffix}");
            }
            super_i += sub_i;

            Console.WriteLine();
            Console.WriteLine($"->{aggregateAlias}: {(aggregateFn(g_pairs))}{aggregateSuffix} (in {sub_i} {keyAlias}s)");

            Console.WriteLine($"  {aggregateAlias.ToUpper()}: {(aggregateFn(super_aggrs))}{aggregateSuffix.ToUpper()} (in {super_i} {keyAlias.ToUpper()}S)");
        }
    }

    public static void CheckAgg(Dictionary<int, double> keyValues, int startIndex, int groupLen,
        ref bool checkResult, Func<Dictionary<int, double>, bool> invalidityPred)
    {
        for (int i = startIndex; i < keyValues.Count; i += groupLen)
        {
            var g_pairs = keyValues.Skip(i).Take(groupLen).ToDictionary(x => x.Key, x => x.Value);

            if (g_pairs.Count < groupLen) break;
            else if (invalidityPred(g_pairs))
            {
                checkResult = true;
                break;
            }
        }
    }
}


public static class GroupCombinator
{
    public static void PrintCombinations(Dictionary<int, double> keyValues, int minGroupLen, int maxGroupLen,
        string keyAlias, string valueAlias, string valueSuffix, string aggregateAlias, string aggregateSuffix,
        Func<Dictionary<int, double>, double> aggregateFn, bool checkInvalidity,
        Func<Dictionary<int, double>, bool> invalidityPred, int? startKey = null)
    {
        var fnCombinate = (int startIndex, int groupLen) =>
        {
            Console.WriteLine("############################################");
            Console.WriteLine();
            Console.WriteLine($"[GROUP_SIZE: {groupLen}; START_{keyAlias.ToUpper()}: {keyValues.Keys.ElementAt(startIndex)}]");

            bool conditionResult = false;

            GroupHelper.CheckAgg(keyValues, startIndex, groupLen,
                ref conditionResult, invalidityPred);

            if (checkInvalidity && conditionResult)
                System.Console.WriteLine("<skipped>!\n");
            else
                GroupHelper.Print(keyValues, startIndex, groupLen, keyAlias,
                    valueAlias, valueSuffix, aggregateAlias, aggregateSuffix, aggregateFn);
        };

        int start_index_pick = keyValues.Keys.ToList().IndexOf(startKey ?? 0);

        for (int groupLen = minGroupLen; groupLen <= maxGroupLen; groupLen++)
        {
            if (start_index_pick != -1)
                fnCombinate(start_index_pick, groupLen);
            else
                for (int startIndex = 0; startIndex < groupLen; startIndex++)
                    fnCombinate(startIndex, groupLen);
        }
    }

}
