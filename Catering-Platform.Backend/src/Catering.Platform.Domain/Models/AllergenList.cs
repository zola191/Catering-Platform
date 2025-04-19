namespace Catering.Platform.Domain.Models;

public record AllergenList
{
    private AllergenList()
    {
        
    }
    
    public List<string> Items { get; } = [];
    public AllergenList(List<string> items)
    {
        Items = items;
    }
    
    public AllergenList UpdateItems(List<string> newItems)
    {
        return new AllergenList(newItems);
    }
    
    
}