using System.Text;
using Application.Caching;
using LanguageExt.UnsafeValueAccess;
using UnsafeValueAccessExtensions = LanguageExt.UnsafeValueAccess.UnsafeValueAccessExtensions;

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
        if(request.Categories is not null) keyBuilder.Append(":c=").Append(string.Join(",", request.Categories));

        if (request.MinPrice.IsSome && request.MaxPrice.IsSome)
        {
            keyBuilder
                .Append(":min=").Append(request.MinPrice.Value())
                .Append(":max=").Append(request.MaxPrice.Value());
        }

        return keyBuilder.ToString();
    }
}