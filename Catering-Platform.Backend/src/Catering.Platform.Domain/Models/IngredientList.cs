namespace Catering.Platform.Domain.Models;

public record IngredientList
{
    private IngredientList()
    {
        
    }

    public List<string> Items { get; } = [];
    public IngredientList(List<string> items)
    {
        Items = items;
    }
    
    public IngredientList UpdateItems(List<string> newItems)
    {
        return new IngredientList(newItems);
    }
}