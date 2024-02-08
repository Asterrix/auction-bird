using System;
using System.Collections.Generic;
using System.Linq;
using Application.Search;
using JetBrains.Annotations;
using Xunit;

namespace Application.UnitTests.Search;

[TestSubject(typeof(SearchAlgorithm))]
public class SearchAlgorithmTest
{
    [Fact(DisplayName = "CalculateSimilarity: Should return the most similar string")]
    public void CalculateSimilarity_ShouldReturnTheMostSimilarString()
    {
        IEnumerable<string> source = new List<string> { "apple", "banana", "orange" };
        const string target = "aple";
        const string expected = "apple";

        string result = SearchAlgorithm.CalculateSimilarity(source, target);

        Assert.Equal(expected, result);
    }

    [Fact(DisplayName =
        "CalculateSimilarity: Should return the most similar string when multiple similar strings exist")]
    public void CalculateSimilarity_ShouldReturnTheMostSimilarStringWithMultipleSimilarStrings()
    {
        IEnumerable<string> source = new List<string> { "apple", "apricot", "aprikot", "avocado", "banana", "orange" };
        const string target = "apricoot";
        const string expected = "apricot";

        string result = SearchAlgorithm.CalculateSimilarity(source, target);

        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "CalculateSimilarityIgnoreCase: Should return the most similar string ignoring case")]
    public void CalculateSimilarityIgnoreCase_ShouldReturnTheMostSimilarStringIgnoringCase()
    {
        IEnumerable<string> source = new List<string> { "apple", "Apple", "orange" };
        const string target = "Apple";
        const string expected = "apple";

        string result = SearchAlgorithm.CalculateSimilarityIgnoreCase(source, target);

        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "CalculateSimilarityIgnoreCase: Should return 'pineapple'")]
    public void CalculateSimilarityIgnoreCase_ShouldReturnPineappleForTheInputPApple()
    {
        IEnumerable<string> source = new List<string> { "apple", "banana", "pineapple", "orange" };
        const string target = "p napple";
        const string expected = "pineapple";

        string result = SearchAlgorithm.CalculateSimilarityIgnoreCase(source, target);

        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "CalculateSimilarityIgnoreCase: Should return the most similar string")]
    public void CalculateSimilarityIgnoreCase_ShouldReturnTheMostSimilarString()
    {
        IEnumerable<string> source = new List<string>
        {
            "Exquisite Crystal-Encrusted Vintage Chandelier Pendant Light Fixture",
            "Magnificent Crystal-Encrusted Vintage Chandelier Pendant Light Fixture",
            "Grandiose Crystal-Encrusted Vintage Chandelier Pendant Light Fixture"
        };

        const string target = "magni rySl-x-enKruse vontage chendelierpendant loight faxture";
        string suggested = SearchAlgorithm.CalculateSimilarityIgnoreCase(source, target);

        Assert.Equal(source.ElementAt(1), suggested, StringComparer.OrdinalIgnoreCase);
    }
}