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

    const fetchTodos = async () => {
        $('#loading').removeClass('hidden');
        try {
            const response = await fetch('/api/todos');
            if (!response.ok) {
                throw new Error('Failed to fetch todos.');
            }
            const res = await response.json();
            if (res.success) {
                renderTodos(res.data);
            }
        } catch (error) {
            showError(error.message);
        } finally {
            $('#loading').addClass('hidden');
        }
    };

    const renderTodos = (todos) => {
        $('#todo-item-list').empty();
        todos.forEach(todo => {
            const todoElement = $(`
                <li class="flex justify-between items-center mt-2 p-2 border rounded-lg">
                    <div class="flex items-center">
                        <input type="checkbox" class="complete-todo-item mr-2 h-5 w-5 text-blue-600" data-id="${todo.id}" ${todo.isDone ? 'checked' : ''}>
                        <span class="todo-item-text ${todo.isDone ? 'line-through' : ''}">${todo.text}</span>
                        <span class="ml-2 text-sm text-gray-500">[${todo.priority}]</span>
                    </div>
                    <button class="delete-todo-item text-red-500" data-id="${todo.id}">Delete</button>
                </li>
            `);
            $('#todo-item-list').append(todoElement);
        });
    };

    const addTodo = async () => {
        const newTodoText = $('#new-todo-item-text').val().trim();
        const newTodoPriority = $('#new-todo-item-priority').val();
        if (newTodoText.length > 50) {
            showError('Character length should not exceed 50.');
            return;
        }
        try {
            const response = await fetch('/api/todos', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ text: newTodoText, priority: newTodoPriority })
            });
            if (!response.ok) {
                throw new Error('Failed to add todo.');
            }
            const res = await response.json();
            if (res.success) {
                fetchTodos();
                showSuccess('Added Successfully.');
                $('#new-todo-item-text').val('');
                $('#new-todo-item-priority').val('0');
            } else {
                showError(res.errorMessage);
            }
        } catch (error) {
            showError(error.message);
        }
    };

    const updateTodo = async (todoId, isDone) => {
        try {
            const response = await fetch(`/api/todos/${todoId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ isDone })
            });
            if (!response.ok) {
                throw new Error('Failed to update todo.');
            }
            const todoText = $(`input[data-id="${todoId}"]`).siblings('.todo-item-text');
            if (isDone) {
                todoText.addClass('line-through');
            } else {
                todoText.removeClass('line-through');
            }
        } catch (error) {
            showError(error.message);
        }
    };

    const removeTodo = async (todoId) => {
        try {
            const response = await fetch(`/api/todos/${todoId}`, {
                method: 'DELETE'
            });
            if (!response.ok) {
                throw new Error('Failed to delete todo.');
            }
            const res = await response.json();
            if (res.success) {
                fetchTodos();
                showSuccess('Deleted Successfully.');
            } else {
                showError(res.errorMessage);
            }
        } catch (error) {
            showError(error.message);
        }
    };

    $('#add-todo-item').on('click', addTodo);

    $(document).on('change', '.complete-todo-item', function () {
        const todoId = $(this).data('id');
        const isDone = $(this).is(':checked');
        updateTodo(todoId, isDone);
    });

    $(document).on('click', '.delete-todo-item', function () {
        deleteTodoId = $(this).data('id');
        $('#delete-confirmation-modal').removeClass('hidden flex').addClass('flex');
    });

    $('#cancel-delete').on('click', function () {
        $('#delete-confirmation-modal').removeClass('flex').addClass('hidden');
        deleteTodoId = null;
    });

    $('#confirm-delete').on('click', function () {
        if (deleteTodoId) {
            removeTodo(deleteTodoId);
            $('#delete-confirmation-modal').removeClass('flex').addClass('hidden');
            deleteTodoId = null;
        }
    });

    fetchTodos();
});