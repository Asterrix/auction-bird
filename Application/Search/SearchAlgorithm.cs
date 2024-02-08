namespace Application.Search;

public static class SearchAlgorithm
{
    public static string CalculateSimilarity(IEnumerable<string> source, string target)
    {
        string result = string.Empty;
        int minDistance = int.MaxValue;

        foreach (string s in source)
        {
            int distance = CalculateSimilarity(s, target);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = s;
            }
        }

        return result;
    }
    
    public static int CalculateSimilarity(string source, string target)
    {
        int n = source.Length;
        int m = target.Length;
        int[,] dp = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++)
        {
            for (int j = 0; j <= m; j++)
            {
                if (i == 0)
                {
                    dp[i, j] = j;
                }
                else if (j == 0)
                {
                    dp[i, j] = i;
                }
                else if (source[i - 1] == target[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1];
                }
                else
                {
                    dp[i, j] = 1 + Math.Min(dp[i, j - 1], Math.Min(dp[i - 1, j], dp[i - 1, j - 1]));
                }
            }
        }

        return dp[n, m];
    }
    
    public static string CalculateSimilarityIgnoreCase(IEnumerable<string> source, string target)
    {
        return CalculateSimilarity(source.Select(s => s.ToLower()), target.ToLower());
    }
}