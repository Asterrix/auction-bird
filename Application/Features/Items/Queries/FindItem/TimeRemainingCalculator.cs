namespace Application.Features.Items.Queries.FindItem;

public sealed class TimeRemainingCalculator : ITimeRemainingCalculator
{
    public string CalculateTimeRemaining(DateTime endTime)
    {
        TimeSpan timeRemaining = endTime - DateTime.UtcNow;
       
        return timeRemaining switch
        {
            _ when timeRemaining.Days > 0 => $"{timeRemaining.Days} days, {timeRemaining.Hours} hours, {timeRemaining.Minutes} minutes",
            _ when timeRemaining.Hours > 0 => $"{timeRemaining.Hours} hours, {timeRemaining.Minutes} minutes",
            _ when timeRemaining.Minutes > 0 => $"{timeRemaining.Minutes} minutes",
            _ when timeRemaining.Seconds > 0 => $"{timeRemaining.Seconds} seconds",
            _ => "Auction has ended"
        };
    }
}