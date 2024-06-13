using System.ComponentModel.DataAnnotations;
using AgendaManager.Infrastructure;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgendaManager.Pages;

public class Agenda(IDocumentStore store) : PageModel
{
    [BindProperty(SupportsGet = true, BinderType = typeof(EncryptedParameter))]
    public Guid Id { get; set; } = default!;

    public string DisplayName { get; set; } = "";

    [BindProperty, Required]
    public string? Name { get; set; }

    [BindProperty]
    public List<Models.Item> Items { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        await using var session = store.LightweightSession();
        var record =  await session.LoadAsync<Models.Agenda>(Id);
        
        if (record is null) return NotFound();
        
        Items = record.Items;
        DisplayName = Name = record.Name;

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await using var session = store.DirtyTrackedSession();
        var record = await session.LoadAsync<Models.Agenda>(Id);

        if (record is null) return NotFound();

        DisplayName = record.Name;
        
        // Clear empty ones
        Items.RemoveAll(i => string.IsNullOrWhiteSpace(i.Text));
        
        if (ModelState.IsValid)
        {
            record.Name = Name!;
            record.Items.Clear();
            record.Items.AddRange(Items);
            
            await session.SaveChangesAsync();

            return RedirectToPage("Agenda", new { id = record.Id });
        }

        return Page();
    }
}