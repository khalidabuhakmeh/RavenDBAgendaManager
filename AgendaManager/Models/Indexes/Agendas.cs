using Raven.Client.Documents.Indexes;

namespace AgendaManager.Models.Indexes;

public class Agendas : AbstractIndexCreationTask<Agenda>
{
    public Agendas()
    {
        Map = agendas =>
            from agenda in agendas
            select new { agenda.Name, agenda.Created };
    }
}