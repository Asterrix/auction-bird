using System.Text;
using Application.Caching;

namespace Application.Features.Items.Queries.ListItems;

public sealed class ListItemsCacheKeyBuilder : ICacheKeyBuilder<ListItemsQuery>
{
    public string BuildKey(ListItemsQuery request)
    {
        StringBuilder keyBuilder = new();
        keyBuilder
            .Append("list_items")
            .Append(":p=").Append(request.Pageable.Page)
            .Append(":s=").Append(request.Pageable.Size);

        if (request.Search is not null) keyBuilder.Append(":q=").Append(request.Search);

        return keyBuilder.ToString();
    }
}