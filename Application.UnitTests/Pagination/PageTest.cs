using System.Collections.Generic;
using Application.Pagination;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Application.UnitTests.Pagination;

[TestSubject(typeof(Page))]
public class PageTest
{
    [Fact(DisplayName = "IsEmpty: When list of elements is empty, return true")]
    public void IsEmpty_WhenListOfElementsIsEmpty_ReturnTrue()
    {
        IEnumerable<string> elements = [];
        Pageable pageable = Pageable.Of(1, 3);
        const int totalElements = 0;

        Page<string> page = new(ref elements, pageable, totalElements);

        Assert.True(page.IsEmpty);
    }

    [Fact(DisplayName = "TotalPages: When total elements is 5 and page size is 3, return 2")]
    public void TotalPages_WhenTotalElementsIs5AndPageSizeIs3_Return2()
    {
        IEnumerable<string> elements = ["a", "b", "c"];
        Pageable pageable = Pageable.Of(1, 3);
        const int totalElements = 5;

        Page<string> page = new(ref elements, pageable, totalElements);

        Assert.Equal(2, page.TotalPages);
    }
    
    [Fact(DisplayName = "IsLastPage: When page number is 2 and total pages is 2, return true")]
    public void IsLastPage_WhenPageNumberIs2AndTotalPagesIs2_ReturnTrue()
    {
        IEnumerable<string> elements = ["a", "b", "c"];
        Pageable pageable = Pageable.Of(2, 3);
        const int totalElements = 5;

        Page<string> page = new(ref elements, pageable, totalElements);

        Assert.True(page.IsLastPage);
    }
    
    [Fact(DisplayName = "IsLastPage: When page number is 1 and total pages is 2, return false")]
    public void IsLastPage_WhenPageNumberIs1AndTotalPagesIs2_ReturnFalse()
    {
        IEnumerable<string> elements = ["a", "b", "c"];
        Pageable pageable = Pageable.Of(1, 3);
        const int totalElements = 5;

        Page<string> page = new(ref elements, pageable, totalElements);

        Assert.False(page.IsLastPage);
    }
}