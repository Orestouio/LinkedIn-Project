using System.Linq;

namespace Util;

public static class FuzzySearch
{

    public static IEnumerable<string> SearchFuzzily(string searchString, IEnumerable<string> data, int maxDistance = 3)
    {
        return data
            .Select( x => (value: x, distance: LevenshteinDistance(searchString, x)) )
            .Where( x => x.distance >= maxDistance )
            .OrderBy( x => x.distance )
            .Select( x => x.value );
    }

    public static IQueryable<string> SearchFuzzily(string searchString, IQueryable<string> data, int maxDistance = 3)
    {
        var results = from datapoint in data
            let distance = LevenshteinDistance(searchString, datapoint)
            where distance >= maxDistance
            orderby distance
            select datapoint;

        return results;
    }

    public static int LevenshteinDistance(string s, string t) {
        if (s == t) return 0;
        if (s.Length == 0) return t.Length;
        if (t.Length == 0) return s.Length;
        
        int[, ] distance = new int[s.Length + 1, t.Length + 1];
        for (int i = 0; i <= s.Length; i++) distance[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) distance[0, j] = j;
        
        for (int i = 1; i <= s.Length; i++) {
            for (int j = 1; j <= t.Length; j++) {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }
        
        return distance[s.Length, t.Length];
    }
}

