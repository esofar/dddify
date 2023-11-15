using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Todolist.Infrastructure.EntityConfigurations;

public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.ToTable(nameof(TodoList));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(c => c.Colour, a =>
        {
            a.Property(c => c.Code).HasMaxLength(20).IsRequired(false);
        });

        builder.HasMany(c => c.Items)
            .WithOne(c => c.TodoList)
            .OnDelete(DeleteBehavior.NoAction)
            .HasForeignKey(c => c.TodoListId)
            .Metadata.DependentToPrincipal!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(
           new
           {
               Id = new Guid("08daa6de-c742-4197-8fdc-eb883ba3b4ec"),
               Title = "Todo List",
               IsDeleted = false,
           });
    }
}
