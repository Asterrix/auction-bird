namespace Application.Features.Items;

public static class ItemConstraint
{
    public const int MinNameLength = 3;
    public const int MaxNameLength = 64;

    public const int MinDescriptionLength = 20;
    public const int MaxDescriptionLength = 700;

    public const decimal MinInitialPrice = decimal.One;
    public const decimal MaxInitialPrice = 1_000_000m;
}