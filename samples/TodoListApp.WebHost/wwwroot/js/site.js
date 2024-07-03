// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    const showError = (message) => {
        $('#error-msg').text(message).fadeIn();
        setTimeout(() => {
            $('#error-msg').fadeOut();
        }, 2000);
    };

    const showSuccess = (message) => {
        $('#success-msg').text(message).fadeIn();
        setTimeout(() => {
            $('#success-msg').fadeOut();
        }, 2000);
    };

    const fetchTodoItems = async () => {
        $('#loading').removeClass('hidden');
        try {
            const response = await fetch('/api/todo-items');
            if (!response.ok) {
                throw new Error('Failed to fetch todo items.');
            }
            const res = await response.json();
            const todoItems = res.success ? res.data: [];
            renderTodoItems(todoItems);
        } catch (error) {
            showError(error.message);
        } finally {
            $('#loading').addClass('hidden');
        }
    };

    const renderTodoItems = (todoItems) => {
        $('#todo-item-list').empty();
        todoItems.forEach(todoItem => {
            const todoItemElement = $(`
                <li class="flex justify-between items-center mt-2 p-2 border rounded-lg">
                    <div class="flex items-center">
                        <input type="checkbox" class="complete-todo-item mr-2 h-5 w-5 text-blue-600" data-id="${todoItem.id}" ${todoItem.isDone ? 'checked' : ''}>
                        <span class="todo-item-text ${todoItem.isDone ? 'line-through' : ''}">${todoItem.text}</span>
                        <span class="ml-2 text-sm text-gray-500">[${todoItem.priorityLevel}]</span>
                    </div>
                    <button class="delete-todo-item text-red-500" data-id="${todoItem.id}">Delete</button>
                </li>
            `);
            $('#todo-item-list').append(todoItemElement);
        });
    };

    const addTodoItem = async () => {
        const newTodoItemText = $('#new-todo-item-text').val().trim();
        const newTodoItemPriority = $('#new-todo-item-priority').val();
        if (newTodoItemText === '') {
            showError('Please enter text.');
            return;
        }
        if (newTodoItemText.length > 50) {
            showError('Character length should not exceed 50.');
            return;
        }
        try {
            const response = await fetch('/api/todo-items', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ text: newTodoItemText, priorityLevel: newTodoItemPriority })
            });
            if (!response.ok) {
                throw new Error('Failed to add todo item.');
            }
            const res = await response.json();
            if (res.success) {
                fetchTodoItems();
                showSuccess('Added Successfully.');
                $('#new-todo-item-text').val('');
                $('#new-todo-item-priority').val('low');
            } else {
                showError(res.errorMessage);
            }
        } catch (error) {
            showError(error.message);
        }
    };

    const updateTodoItem = async (todoItemId, isDone) => {
        try {
            const response = await fetch(`/api/todo-items/${todoItemId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ isDone })
            });
            if (!response.ok) {
                throw new Error('Failed to update todo item.');
            }
            const todoItemText = $(`input[data-id="${todoItemId}"]`).siblings('.todo-item-text');
            if (isDone) {
                todoItemText.addClass('line-through');
            } else {
                todoItemText.removeClass('line-through');
            }
        } catch (error) {
            showError(error.message);
        }
    };

    const removeTodoItem = async (todoItemId) => {
        try {
            const response = await fetch(`/api/todo-items/${todoItemId}`, {
                method: 'DELETE'
            });
            if (!response.ok) {
                throw new Error('Failed to delete todo item.');
            }
            const res = await response.json();
            if (res.success) {
                fetchTodoItems();
                showSuccess('Deleted Successfully.');
            } else {
                showError(res.errorMessage);
            }
        } catch (error) {
            showError(error.message);
        }
    };

    $('#add-todo-item').on('click', addTodoItem);

    $(document).on('change', '.complete-todo-item', function () {
        const todoItemId = $(this).data('id');
        const isDone = $(this).is(':checked');
        updateTodoItem(todoItemId, isDone);
    });

    $(document).on('click', '.delete-todo-item', function () {
        deleteTodoItemId = $(this).data('id');
        $('#delete-confirmation-modal').removeClass('hidden flex').addClass('flex');
    });

    $('#cancel-delete').on('click', function () {
        $('#delete-confirmation-modal').removeClass('flex').addClass('hidden');
        deleteTodoItemId = null;
    });

    $('#confirm-delete').on('click', function () {
        if (deleteTodoItemId) {
            removeTodoItem(deleteTodoItemId);
            $('#delete-confirmation-modal').removeClass('flex').addClass('hidden');
            deleteTodoItemId = null;
        }
    });

    fetchTodoItems();
});