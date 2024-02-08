namespace Application.Pagination;

public sealed class Pageable
{
    // Constraints
    private const int MinPageNumber = 1;
    private const int MinPageSize = 1;

    private const int MaxPageSize = 64;

    // Default values
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 9;

    // Properties
    public int Page { get; }
    public int Size { get; }
    public int Skip => (Page - 1) * Size;

    private Pageable(int page, int size)
    {
        Page = page;
        Size = size;
    }

    public static Pageable Of(int pageNumber, int pageSize)
    {
        if (pageNumber < MinPageNumber)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                pageNumber,
                $"Page number must be greater than or equal to {MinPageNumber}");
        }

        if (pageSize < MinPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                $"Page size must be greater than or equal to {MinPageSize}");
        }

        if (pageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                $"Page size must be less than or equal to {MaxPageSize}");
        }

        return new Pageable(pageNumber, pageSize);
    }

    public static Pageable Of(int pageNumber)
    {
        return Of(pageNumber, DefaultPageSize);
    }

    public static Pageable Of()
    {
        return Of(DefaultPageNumber, DefaultPageSize);
    }
}