using System.ComponentModel.DataAnnotations;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgendaManager.Pages;

public class IndexModel(
    ILogger<IndexModel> logger,
    IDocumentStore documentStore) : PageModel
{
    [BindProperty, Required(ErrorMessage = "Agenda name is required")]
    public string? Name { get; set; }

    public IReadOnlyList<Models.Agenda> Results { get; set; } = new List<Models.Agenda>();

    private async Task<IReadOnlyList<Models.Agenda>> GetResults(IDocumentSession session)
    {
        return await session.Query<Models.Agenda>()
            .OrderBy(a => a.Created)
            .ToListAsync();
    }

    public async Task OnGet()
    {
        await using var session = documentStore.LightweightSession();
        Results = await GetResults(session);
    }
    
    public async Task<IActionResult> OnPost()
    {
        await using var session = documentStore.DirtyTrackedSession();
        
        if (ModelState.IsValid)
        {
            var agenda = new Models.Agenda { Name = Name! };
            session.Store(agenda);
            await session.SaveChangesAsync();
            
            logger.LogInformation("""Saved agenda "{Name}" ({Id}) to database""", agenda.Name, agenda.Id);

            return RedirectToPage("Agenda", new { id = agenda.Id });
        }

        Results = await GetResults(session); 

        return Page();
    }
}