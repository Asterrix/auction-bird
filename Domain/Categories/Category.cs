namespace Domain.Categories;

public sealed class Category
{
    public int Id { get; }
    public string Name { get; set; }
    public Category? Parent { get; set; }
}