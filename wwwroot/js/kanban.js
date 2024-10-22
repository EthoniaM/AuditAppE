document.addEventListener('DOMContentLoaded', () => {
    let draggedTask = null;

    window.toggleDropdown = function (event, element) {
        event.stopPropagation();
        const dropdownMenu = element.nextElementSibling;
        dropdownMenu.classList.toggle('show');
    };

    document.addEventListener('click', function (event) {
        const dropdowns = document.querySelectorAll('.dropdown-menu');
        dropdowns.forEach(function (dropdown) {
            if (!dropdown.contains(event.target) && !dropdown.previousElementSibling.contains(event.target)) {
                dropdown.classList.remove('show');
            }
        });
    });

    const columns = document.querySelectorAll('.kanban-column');

    columns.forEach(column => {
        column.addEventListener('dragover', (e) => {
            e.preventDefault();
            column.classList.add('drag-over');
        });

        column.addEventListener('dragleave', () => {
            column.classList.remove('drag-over');
        });

        column.addEventListener('drop', async (e) => {
            e.preventDefault();
            column.classList.remove('drag-over');

            if (draggedTask) {
                const kanbanId = draggedTask.getAttribute('data-kanban-id');
                const newStatus = column.getAttribute('data-status');

                // Update on UI
                draggedTask.parentElement.removeChild(draggedTask);
                column.querySelector('.kanban-tasks').appendChild(draggedTask);
                updateColumnCounts();

                // Send AJAX request to update task status on the server
                try {
                    const response = await fetch('/KanbanBoard/UpdateStatus', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify({
                            KanBanID: kanbanId,
                            Status: newStatus,
                        }),
                    });

                    const result = await response.json();

                    if (!result.success) {
                        console.error('Failed to update task status.');
                    }
                } catch (error) {
                    console.error('Error updating task status:', error);
                }

                draggedTask = null;
            }
        });
    });

    document.querySelectorAll('.kanban-task').forEach(task => {
        task.addEventListener('dragstart', (e) => {
            draggedTask = e.target;
            e.dataTransfer.setData('text/plain', e.target.getAttribute('data-kanban-id'));
        });
    });

    // Handle task update
    const updateButtons = document.querySelectorAll('.task-update-btn');
    if (updateButtons.length === 0) {
        console.warn('No update buttons found.');
    }
    updateButtons.forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            const kanbanId = button.getAttribute('data-kanban-id');
            const taskData = {
                KanBanID: kanbanId,
                TaskTitle: button.getAttribute('data-task-title'),
                Status: button.getAttribute('data-task-status'),
                Priority: button.getAttribute('data-task-priority'),
                TaskDescription: button.getAttribute('data-task-description'),
                StartDate: button.getAttribute('data-task-start-date'),
                EndDate: button.getAttribute('data-task-end-date'),
            };

            try {
                const response = await fetch(`/KanbanBoard/UpdateTask`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(taskData),
                });

                const result = await response.json();

                if (!result.success) {
                    console.error('Failed to update task.');
                } else {
                    console.log('Task updated successfully.');
                    // Optionally, update the UI or redirect
                }
            } catch (error) {
                console.error('Error updating task:', error);
            }
        });
    });

    // Handle task deletion
    const deleteButton = document.querySelector('#deleteTaskButton');
    if (deleteButton) {
        deleteButton.addEventListener('click', async (e) => {
            e.preventDefault();
            const kanbanId = document.querySelector('#kanbanId').value;
            const confirmed = confirm('Are you sure you want to delete this task?');
            if (confirmed) {
                try {
                    const response = await fetch(`/KanbanBoard/DeleteTask`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify({ KanBanID: kanbanId }),
                    });

                    const result = await response.json();

                    if (!result.success) {
                        console.error('Failed to delete task.');
                    } else {
                        console.log('Task deleted successfully.');
                        // Redirect or update the UI as needed
                        window.location.href = '/KanbanBoard'; // Redirect to task list page
                    }
                } catch (error) {
                    console.error('Error deleting task:', error);
                }
            }
        });
    } else {
        console.warn('Delete task button not found.');
    }

    // Example of creating a task with auditId
    document.querySelector('#createTaskButton')?.addEventListener('click', async () => {
        const taskData = {
            AuditID: document.querySelector('#auditIdInput').value, // Get the selected AuditID
            TaskTitle: document.querySelector('#taskTitleInput').value,
            Status: document.querySelector('#statusInput').value,
            Priority: document.querySelector('#priorityInput').value,
            TaskDescription: document.querySelector('#taskDescriptionInput').value,
            StartDate: document.querySelector('#startDateInput').value,
            EndDate: document.querySelector('#endDateInput').value,
        };

        try {
            const response = await fetch('/KanbanBoard/CreateTask', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(taskData),
            });

            const result = await response.json();

            if (!result.success) {
                console.error('Failed to create task.');
            } else {
                console.log('Task created successfully.');
                // Optionally, update the UI with the newly created task
            }
        } catch (error) {
            console.error('Error creating task:', error);
        }
    });

    function updateColumnCounts() {
        const notStartedCount = document.querySelectorAll('.kanban-column[data-status="Not Started"] .kanban-task').length;
        const inProgressCount = document.querySelectorAll('.kanban-column[data-status="In Progress"] .kanban-task').length;
        const completedCount = document.querySelectorAll('.kanban-column[data-status="Completed"] .kanban-task').length;

        document.querySelector('.kanban-column[data-status="Not Started"] .status-header').textContent = `Not Started (${notStartedCount})`;
        document.querySelector('.kanban-column[data-status="In Progress"] .status-header').textContent = `In Progress (${inProgressCount})`;
        document.querySelector('.kanban-column[data-status="Completed"] .status-header').textContent = `Completed (${completedCount})`;
    }
});
