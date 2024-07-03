using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoListApp.Domain.Entities;
using TodoListApp.Domain.Enums;

namespace TodoListApp.Infrastructure.EntityConfigurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Text)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.PriorityLevel)
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(c => c.IsDone)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasData(
           new
           {
               Id = new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"),
               Text = "Write project proposal 📃",
               PriorityLevel = PriorityLevel.Low,
               IsDone = false,
               IsDeleted = false,
           },
           new
           {
               Id = new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"),
               Text = "Schedule kick-off meeting ✅",
               PriorityLevel = PriorityLevel.Medium,
               IsDone = false,
               IsDeleted = false,
           },
           new
           {
               Id = new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"),
               Text = "Review research results 🤯",
               PriorityLevel = PriorityLevel.High,
               IsDone = false,
               IsDeleted = false,
           });
    }
}
