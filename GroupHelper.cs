public static class GroupHelper
{
    public static void Print(Dictionary<int, int> keyValues, int startIndex, int groupLen,
         string keyAlias, string valueAlias, string valueSuffix, string aggregateAlias, Func<Dictionary<int, int>, double> aggregateFn)
    {
        for (int i = startIndex; i < keyValues.Count; i += groupLen)
        {
            Console.WriteLine("_______________________________________");
            Console.WriteLine();

            var g_pairs = keyValues.Skip(i).Take(groupLen).ToDictionary(x => x.Key, x => x.Value);

            Console.WriteLine("group-" + (i / groupLen + 1).ToString("00") + ":");

            var sub_i = 0;
            foreach (var kvp in g_pairs)
            {
                sub_i++;
                Console.WriteLine($"{sub_i}.{keyAlias}: {kvp.Key} => {valueAlias}: {kvp.Value}{valueSuffix}");
            }

            Console.WriteLine($"{aggregateAlias}: {aggregateFn(g_pairs)}{valueSuffix}");
            Console.WriteLine();
        }
    }

    public static void CheckAgg(Dictionary<int, int> keyValues, int startIndex, int groupLen,
        ref bool checkResult, Func<Dictionary<int, int>, bool> invalidityPred)
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
    public static void PrintCombinations(Dictionary<int, int> keyValues, int minGroupLen, int maxGroupLen,
        string keyAlias, string valueAlias, string valueSuffix, string aggregateAlias,
        Func<Dictionary<int, int>, double> aggregateFn, bool checkInvalidity,
        Func<Dictionary<int, int>, bool> invalidityPred)
    {
        Console.WriteLine($"TOTAL {valueAlias.ToUpper()} (IN {keyValues.Count} {keyAlias.ToUpper()}S): {aggregateFn(keyValues)}{valueSuffix}");
        Console.WriteLine();

        for (int groupLen = minGroupLen; groupLen <= maxGroupLen; groupLen++)
        {
            for (int startIndex = 0; startIndex < groupLen; startIndex++)
            {
                Console.WriteLine("############################################");
                Console.WriteLine();
                Console.WriteLine($"[GROUP_SIZE: {groupLen}; START_{keyAlias.ToUpper()}: {keyValues.Keys.ElementAt(startIndex)}]");

                bool conditionResult = false;

                GroupHelper.CheckAgg(keyValues, startIndex, groupLen,
                    ref conditionResult, invalidityPred);

                if (checkInvalidity && conditionResult)
                {
                    System.Console.WriteLine("<skipped>!");
                    Console.WriteLine();
                }
                else
                {
                    GroupHelper.Print(keyValues, startIndex, groupLen, keyAlias,
                        valueAlias, valueSuffix, aggregateAlias, aggregateFn);
                }
            }
        }
    }

}
