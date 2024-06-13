using System.ComponentModel.DataAnnotations;
using AgendaManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;

namespace AgendaManager.Pages;

public class Agenda(IDocumentStore store) : PageModel
{
    [BindProperty(SupportsGet = true, BinderType = typeof(EncryptedParameter))]
    public string? Id { get; set; }
    
    public string DisplayName { get; set; }

    [BindProperty, Required]
    public string? Name { get; set; }

    [BindProperty]
    public List<Models.Item> Items { get; set; } = new();

    public async Task OnGet()
    {
        using var session = store.OpenAsyncSession();
        var result =  await session.LoadAsync<Models.Agenda>(Id);
        
        Items = result.Items;
        DisplayName = Name = result.Name;
    }

    public async Task<IActionResult> OnPost()
    {
        using var session = store.OpenAsyncSession();
        var record = await session.LoadAsync<Models.Agenda>(Id);

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