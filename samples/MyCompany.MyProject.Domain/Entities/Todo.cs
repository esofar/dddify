﻿using Dddify.Auditing;
using Dddify.Domain.Entities;
using MyCompany.MyProject.Domain.DomainEvents;
using MyCompany.MyProject.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace MyCompany.MyProject.Domain.Entities;

public class Todo : FullAuditedAggregateRoot<Guid>, ISoftDeletable, IHasConcurrencyStamp
{
    public Todo(string title, Colour colour)
    {
        Title = title;
        Colour = colour;

        AddDomainEvent(new TodoCreatedDomainEvent(this));
    }

    public Todo() { }

    public string Title { get; set; }

    public Colour Colour { get; set; }

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();

    public bool IsDeleted { get; set; }

    public string ConcurrencyStamp { get; set; }

    public void AddItem(TodoItem item)
    {
        Items.Add(item);
    }
}
