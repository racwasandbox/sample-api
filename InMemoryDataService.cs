using System.Diagnostics;
using System.Globalization;

namespace SampleApi;

public class InMemoryDataService
{
    private readonly List<Item> _items = new List<Item>();

    public void Add(string label)
    {
        int nextId = _items.Any() ? _items.Last().Id + 1 : 0;

        var item = new Item
        {
            Id = nextId,
            Label = label
        };

        _items.Add(item);
    }

    public Item Get(int id)
    {
        if (id < 0)
        {
            throw new ArgumentException("id cannot be negative");
        }
        
        var item = _items.FirstOrDefault(x => x.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "Item id does not exist");

        return item;
    }

    public void Update(int id, string label)
    {
        if (id < 0)
        {
            throw new ArgumentException("id cannot be negative");
        }
        
        var item = _items.FirstOrDefault(x => x.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "Item id does not exist");
        var newItem = new Item
        {
            Id = item.Id,
            Label = label
        };

        _items[id] = newItem;

    }

    public void Remove(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "Item id does not exist");

        _items.Remove(item);
    }
}

public class Item
{
    public int Id { get; init; }
    public string Label { get; init; } = string.Empty;
}
