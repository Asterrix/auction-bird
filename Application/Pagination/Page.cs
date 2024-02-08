namespace Application.Pagination;

public sealed class Page<T>
{
    public IEnumerable<T> Elements { get; }
    public int TotalElements { get; }
    public int TotalPages { get; }
    public bool IsEmpty { get; }
    public bool IsLastPage { get; }

    public Page(ref readonly IEnumerable<T> elements, int totalElements, int totalPages, bool isEmpty, bool isLastPage)
    {
        Elements = elements;
        TotalElements = totalElements;
        TotalPages = totalPages;
        IsEmpty = isEmpty;
        IsLastPage = isLastPage;
    }
    
    public Page(ref readonly IEnumerable<T> elements, Pageable pageable, int totalElements)
    {
        Elements = elements;
        TotalElements = totalElements;
        TotalPages = (int)Math.Ceiling(totalElements / (double)pageable.Size);
        IsEmpty = !elements.Any();
        IsLastPage = pageable.Page >= TotalPages;
    }
}