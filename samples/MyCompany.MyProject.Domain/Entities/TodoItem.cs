using Dddify.Domain.Entities;
using MyCompany.MyProject.Domain.Enums;

namespace MyCompany.MyProject.Domain.Entities;

public class TodoItem : FullAuditedEntity<Guid>
{
    public string Title { get; set; } = default!;

    public string Note { get; set; } = default!;

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }
}
