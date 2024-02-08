namespace Application.Pagination;

public sealed class Page<T>(
    IEnumerable<T> elements,
    Pageable pageable,
    int totalElements)
{
    public IEnumerable<T> Elements { get; } = elements;
    public bool IsEmpty => !Elements.Any();
    public int TotalPages => (int)Math.Ceiling(totalElements / (double)pageable.Size);
    public int TotalElements => totalElements;
    public bool IsLastPage => pageable.Number >= TotalPages;
}