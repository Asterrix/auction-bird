using Domain.Categories;

namespace Application.Features.Categories.Mapper;

public record ParentCategory(int Id, string Name, List<ChildCategory> Subcategories);

public record ChildCategory(int Id, string Name);

public static class CategoryMapper
{
    public static List<ParentCategory> MapToCategoryList(List<Category> categories)
    {
        Dictionary<int, ParentCategory> dictionary = new();

        foreach (Category category in categories)
            if (category.Parent is null)
            {
                ParentCategory parent = new(category.Id, category.Name, []);
                dictionary.Add(parent.Id, parent);
            }
            else
            {
                ChildCategory child = new(category.Id, category.Name);
                if (dictionary.TryGetValue(category.Parent.Id, out ParentCategory? value))
                {
                    value.Subcategories.Add(child);
                }
                else
                {
                    ParentCategory newParent = new(category.Parent.Id, category.Parent.Name, [child]);
                    dictionary.Add(newParent.Id, newParent);
                }
            }

        return dictionary.Values.ToList();
    }
}