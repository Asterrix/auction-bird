using System;
using Application.Features.Items.Queries.FindItem;
using JetBrains.Annotations;
using Xunit;

namespace Application.UnitTests.Features.Items.Queries.FindItem;

[TestSubject(typeof(TimeRemainingCalculator))]
public class TimeRemainingCalculatorTest
{
    private const int SecondTolerance = 1;

    [Fact(DisplayName = "CalculateTimeRemaining should return days, hours, and minutes when time remaining is greater than 1 day")]
    public void CalculateTimeRemaining_ShouldReturnDaysHoursAndMinutes_WhenTimeRemainingIsGreaterThan1Day()
    {
        TimeRemainingCalculator timeRemainingCalculator = new();
        DateTime endTime = DateTime.UtcNow.AddDays(2).AddSeconds(SecondTolerance);

        string result = timeRemainingCalculator.CalculateTimeRemaining(endTime);

        Assert.Equal("2 days, 0 hours, 0 minutes", result);
    }

    [Fact(DisplayName = "CalculateTimeRemaining should return hours and minutes when time remaining is greater than 1 hour")]
    public void CalculateTimeRemaining_ShouldReturnHoursAndMinutes_WhenTimeRemainingIsGreaterThan1Hour()
    {
        TimeRemainingCalculator timeRemainingCalculator = new();
        DateTime endTime = DateTime.UtcNow.AddHours(2).AddSeconds(SecondTolerance);

        string result = timeRemainingCalculator.CalculateTimeRemaining(endTime);

        Assert.Equal("2 hours, 0 minutes", result);
    }

    [Fact(DisplayName = "CalculateTimeRemaining should return minutes when time remaining is greater than 1 minute")]
    public void CalculateTimeRemaining_ShouldReturnMinutes_WhenTimeRemainingIsGreaterThan1Minute()
    {
        TimeRemainingCalculator timeRemainingCalculator = new();
        DateTime endTime = DateTime.UtcNow.AddMinutes(2).AddSeconds(SecondTolerance);

        string result = timeRemainingCalculator.CalculateTimeRemaining(endTime);

        Assert.Equal("2 minutes", result);
    }

    [Fact(DisplayName = "CalculateTimeRemaining should return seconds when time remaining is greater than 1 second")]
    public void CalculateTimeRemaining_ShouldReturnSeconds_WhenTimeRemainingIsGreaterThan1Second()
    {
        TimeRemainingCalculator timeRemainingCalculator = new();
        DateTime endTime = DateTime.UtcNow.AddSeconds(2 + SecondTolerance);

        string result = timeRemainingCalculator.CalculateTimeRemaining(endTime);

        Assert.Equal("2 seconds", result);
    }

    [Fact(DisplayName = "CalculateTimeRemaining should return 'Auction finished.' when time remaining is less than 1 second")]
    public void CalculateTimeRemaining_ShouldReturnAuctionFinished_WhenTimeRemainingIsLessThan1Second()
    {
        TimeRemainingCalculator timeRemainingCalculator = new();
        DateTime endTime = DateTime.UtcNow.AddSeconds(-SecondTolerance);

        string result = timeRemainingCalculator.CalculateTimeRemaining(endTime);

        Assert.Equal("Auction finished.", result);
    }

    [Fact(DisplayName = "CalculateTimeRemaining should return 'Auction finished.' when time remaining is 0")]
    public void CalculateTimeRemaining_ShouldReturnAuctionFinished_WhenTimeRemainingIs0()
    {
        TimeRemainingCalculator timeRemainingCalculator = new();
        DateTime endTime = DateTime.UtcNow.AddSeconds(-SecondTolerance);

        string result = timeRemainingCalculator.CalculateTimeRemaining(endTime);

        Assert.Equal("Auction finished.", result);
    }
}