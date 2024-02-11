using System.Collections.Generic;
using System.Linq;
using Application.Specification;
using JetBrains.Annotations;
using Xunit;

namespace Application.UnitTests.Specification;

[TestSubject(typeof(Specification<>))]
public class SpecificationTest
{
    [Fact(DisplayName = "And: return specified element that matches the expression")]
    public void And_WhenCalled_ReturnSpecifiedElementThatMatchesTheExpression()
    {
        const string lookupVal = "x";
        Specification<string> specification = Specification<string>
            .Create()
            .And(s => s == lookupVal);
        List<string> list = ["x", "y", "z", "xx", "yx", "zx"];

        IEnumerable<string> filteredList = list.Where(specification.SpecificationExpression.Compile()).ToList();

        Assert.Single(filteredList);
        Assert.Contains(lookupVal, filteredList);
    }

    [Fact(DisplayName = "And: when chained with another And, return specified element that matches both expressions")]
    public void And_WhenChainedWithAnotherAnd_ReturnSpecifiedElementThatMatchesBothExpressions()
    {
        Specification<string> specification = Specification<string>
            .Create()
            .And(s => s.Length >= 1)
            .And(s => s.Length <= 3);
        List<string> list = ["a", "ab", "abc", "abcd", "abcde"];

        IEnumerable<string> filteredList = list.Where(specification.SpecificationExpression.Compile()).ToList();

        Assert.Equal(3, filteredList.Count());
        Assert.Contains("a", filteredList);
        Assert.Contains("ab", filteredList);
        Assert.Contains("abc", filteredList);
    }

    [Fact(DisplayName = "AndNot: return specified element that matches the expression")]
    public void AndNot_WhenCalled_ReturnSpecifiedElementThatMatchesTheExpression()
    {
        const string lookupVal = "a";
        Specification<string> specification = Specification<string>
            .Create()
            .AndNot(s => s == lookupVal);
        List<string> list = ["a", "b", "c", "d", "f", "g"];

        IEnumerable<string> filteredList = list.Where(specification.SpecificationExpression.Compile()).ToList();

        Assert.Equal(list.Count - 1, filteredList.Count());
        Assert.DoesNotContain(lookupVal, filteredList);
    }

    [Fact(DisplayName = "Or: return specified element that matches the expression")]
    public void Or_WhenCalled_ReturnSpecifiedElementThatMatchesTheExpression()
    {
        const string lookupValOne = "a";
        const string lookupValTwo = "b";
        Specification<string> specification = Specification<string>
            .Create()
            .And(s => s == lookupValOne)
            .Or(s => s == lookupValTwo);
        List<string> list = ["a", "b", "c", "d", "f", "g"];

        IEnumerable<string> filteredList = list.Where(specification.SpecificationExpression.Compile()).ToList();

        Assert.Equal(2, filteredList.Count());
        Assert.Contains(lookupValOne, filteredList);
        Assert.Contains(lookupValTwo, filteredList);
    }
    
    [Fact(DisplayName = "OrNot: ignore specified element that matches the expression")]
    public void OrNot_WhenCalled_ReturnSpecifiedElementThatDoesNotMatchTheExpression()
    {
        const string lookupVal = "a";
        Specification<string> specification = Specification<string>
            .Create()
            .And(s => s == lookupVal)
            .OrNot(s => s == "b");
        List<string> list = ["a", "b", "c", "d", "f", "g"];

        IEnumerable<string> filteredList = list.Where(specification.SpecificationExpression.Compile()).ToList();

        Assert.Equal(list.Count - 1, filteredList.Count());
        Assert.DoesNotContain("b", filteredList);
    }

    [Fact(DisplayName = "Not: ignore specified element that matches the expression")]
    public void Not_WhenCalled_ReturnSpecifiedElementThatDoesNotMatchTheExpression()
    {
        Specification<string> specification = Specification<string>
            .Create()
            .And(x => x == "a")
            .Not();
        
        List<string> list = ["a", "b", "c", "d", "f", "g"];

        IEnumerable<string> filteredList = list.Where(specification.SpecificationExpression.Compile()).ToList();

        Assert.Equal(list.Count - 1, filteredList.Count());
        Assert.DoesNotContain("a", filteredList);
    }
}