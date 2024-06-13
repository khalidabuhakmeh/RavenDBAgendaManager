namespace AgendaManager.Models;

public class Agenda
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<Item> Items { get; set; } = new();
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
}

public class Item
{
    public string? Text { get; set; } = "";
    public bool Done { get; set; } = false;
}