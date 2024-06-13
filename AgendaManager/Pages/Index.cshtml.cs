using System.ComponentModel.DataAnnotations;
using AgendaManager.Models;
using AgendaManager.Models.Indexes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AgendaManager.Pages;

public class IndexModel(
    ILogger<IndexModel> logger,
    IDocumentStore documentStore) : PageModel
{
    [BindProperty, Required(ErrorMessage = "Agenda name is required")]
    public string? Name { get; set; }

    public List<Models.Agenda> Results { get; set; } = new();

    private async Task<List<Models.Agenda>> GetResults(IAsyncDocumentSession session)
    {
        return await session.Query<Models.Agenda, Agendas>()
            .OrderBy(a => a.Created)
            .ToListAsync();
    }

    public async Task OnGet()
    {
        using var session = documentStore.OpenAsyncSession();
        Results = await GetResults(session);
    }
    
    public async Task<IActionResult> OnPost()
    {
        using var session = documentStore.OpenAsyncSession();
        
        if (ModelState.IsValid)
        {
            var agenda = new Models.Agenda { Name = Name! };
            await session.StoreAsync(agenda);
            await session.SaveChangesAsync();
            
            logger.LogInformation("""Saved agenda "{Name}" ({Id}) to database""", agenda.Name, agenda.Id);

            return RedirectToPage("Agenda", new { id = agenda.Id });
        }

        Results = await GetResults(session); 

        return Page();
    }
}