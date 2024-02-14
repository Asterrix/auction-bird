namespace Application.Features.Items.Queries.FindItem;

public interface ITimeRemainingCalculator
{
    string CalculateTimeRemaining(DateTime endTime);
}