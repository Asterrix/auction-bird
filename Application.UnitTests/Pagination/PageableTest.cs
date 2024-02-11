using System;
using Application.Pagination;
using JetBrains.Annotations;
using Xunit;

namespace Application.UnitTests.Pagination;

[TestSubject(typeof(Pageable))]
public class PageableTest
{
    [Fact(DisplayName = "Of: When page number is less than minimum, throw ArgumentOutOfRangeException")]
    public void Of_WhenPageNumberIsLessThanMinimum_ThrowArgumentOutOfRangeException()
    {
        const int pageNumber = 0;

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => Pageable.Of(pageNumber));
        Assert.Contains("Page number must be greater than or equal to", exception.Message);
    }

    [Fact(DisplayName = "Of: When page size is less than minimum, throw ArgumentOutOfRangeException")]
    public void Of_WhenPageSizeIsLessThanMinimum_ThrowArgumentOutOfRangeException()
    {
        const int pageSize = 0;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            const int pageNumber = 1;

            return Pageable.Of(pageNumber, pageSize);
        });

        Assert.Contains("Page size must be greater than or equal to", exception.Message);
    }

    [Fact(DisplayName = "Of: When page size is greater than maximum, throw ArgumentOutOfRangeException")]
    public void Of_WhenPageSizeIsGreaterThanMaximum_ThrowArgumentOutOfRangeException()
    {
        const int pageSize = 65;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            const int pageNumber = 1;

            return Pageable.Of(pageNumber, pageSize);
        });
        
        Assert.Contains("Page size must be less than or equal to", exception.Message);
    }

    [Fact(DisplayName = "Of: When page number and page size are valid, return Pageable")]
    public void Of_WhenPageNumberAndPageSizeAreValid_ReturnPageable()
    {
        const int pageNumber = 1;
        const int pageSize = 9;

        Pageable pageable = Pageable.Of(pageNumber, pageSize);

        Assert.Equal(pageNumber, pageable.Page);
        Assert.Equal(pageSize, pageable.Size);
    }

    [Fact(DisplayName = "Of: When page number is valid and page size is not provided, return Pageable")]
    public void Of_WhenPageNumberIsValidAndPageSizeIsNotProvided_ReturnPageable()
    {
        const int pageNumber = 1;

        Exception exception = Record.Exception(() => Pageable.Of(pageNumber));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "Of: When page number and page size are not provided, return Pageable")]
    public void Of_WhenPageNumberAndPageSizeAreNotProvided_ReturnPageable()
    {
        Exception exception = Record.Exception(Pageable.Of);

        Assert.Null(exception);
    }

    [Fact(DisplayName = "Skip: When page number and page size are valid, return correct skip value")]
    public void Skip_WhenPageNumberAndPageSizeAreValid_ReturnCorrectSkipValue()
    {
        const int pageNumber = 2;
        const int pageSize = 10;
        const int expectedSkip = 10;

        Pageable pageable = Pageable.Of(pageNumber, pageSize);

        int actualSkip = pageable.Skip;
        
        Assert.Equal(expectedSkip, actualSkip);
    }
}