using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todolist.Domain.Entities;

namespace Todolist.Infrastructure.EntityConfigurations;

public class TodoListItemConfiguration : IEntityTypeConfiguration<TodoListItem>
{
    public void Configure(EntityTypeBuilder<TodoListItem> builder)
    {
        builder.ToTable(nameof(TodoListItem));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Note)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Priority)
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.HasData(
           new
           {
               Id = new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"),
               TodoListId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
               Note = "Make a todo list 📃",
               Priority = TodoListItemPriority.Low,
               Order = 1,
               IsDeleted = false,
           },
           new
           {
               Id = new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"),
               TodoListId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
               Note = "Check off the first item ✅",
               Priority = TodoListItemPriority.Medium,
               Order = 2,
               IsDeleted = false,
           },
           new
           {
               Id = new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"),
               TodoListId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
               Note = "Realise you've already done two things on the list! 🤯",
               Priority = TodoListItemPriority.High,
               Order = 3,
               IsDeleted = false,
           },
           new
           {
               Id = new Guid("08daf207-8abf-4fff-830f-93e48ed9a34c"),
               TodoListId = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
               Note = "Reward yourself with a nice, long nap 🏆",
               Priority = TodoListItemPriority.None,
               Order = 4,
               IsDeleted = false,
           });
    }
}
