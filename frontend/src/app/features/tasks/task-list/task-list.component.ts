// 10-11-2026 Construido por: Ing GAAE - Kanban board with drag and drop, edit modal and bitacoras.
import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { CdkDrag, CdkDragDrop, CdkDragHandle, CdkDropList, CdkDropListGroup } from '@angular/cdk/drag-drop';
import { TaskActivity, TaskItem, TaskStatus, TaskStatusHistory, UpdateTaskRequest, UpdateTaskStatusRequest } from '../../../core/models/task.model';
import { User } from '../../../core/models/user.model';
import { AppEventsService } from '../../../core/services/app-events.service';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { extractErrorMessage } from '../../../core/utils/error-message';
import { AlertComponent } from '../../../shared/components/alert/alert.component';
import { SelectComponent, SelectOption } from '../../../shared/components/select/select.component';

@Component({
  selector: 'app-task-list',
  imports: [FormsModule, DatePipe, CdkDropList, CdkDropListGroup, CdkDrag, CdkDragHandle, SelectComponent, AlertComponent],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent implements OnInit {
  private static readonly priorityLabels: Record<string, string> = {
    Low: 'Baja',
    Medium: 'Media',
    High: 'Alta',
  };

  private readonly taskService = inject(TaskService);
  private readonly userService = inject(UserService);
  private readonly events = inject(AppEventsService);

  readonly tasks = signal<TaskItem[]>([]);
  readonly users = signal<User[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly userFilter = signal<string>('');
  readonly sortOrder = signal<'asc' | 'desc'>('desc');

  // Observation modal (drag and drop status change)
  readonly observationModal = signal<{ task: TaskItem; targetStatus: TaskStatus } | null>(null);
  readonly observationText = signal('');

  // Edit modal (edit task + bitacoras)
  readonly editModal = signal<{
    task: TaskItem;
    title: string;
    description: string;
    assignedUserId: number;
    priority: string;
    history: TaskStatusHistory[];
    activities: TaskActivity[];
    loading: boolean;
    newActivity: string;
    saving: boolean;
  } | null>(null);

  readonly pendingTasks = computed(() => this.tasks().filter((t) => t.status === 'Pending'));
  readonly inProgressTasks = computed(() => this.tasks().filter((t) => t.status === 'InProgress'));
  readonly doneTasks = computed(() => this.tasks().filter((t) => t.status === 'Done'));

  readonly counts = computed(() => {
    const tasks = this.tasks();
    return {
      total: tasks.length,
      pending: tasks.filter((t) => t.status === 'Pending').length,
      inProgress: tasks.filter((t) => t.status === 'InProgress').length,
      done: tasks.filter((t) => t.status === 'Done').length,
    };
  });

  readonly userOptions = computed<SelectOption[]>(() => [
    { value: '', label: 'Todos los usuarios' },
    ...this.users().map((user) => ({ value: String(user.id), label: user.name })),
  ]);

  readonly editUserOptions = computed<SelectOption[]>(() =>
    this.users().map((user) => ({ value: String(user.id), label: user.name })),
  );

  readonly sortOptions: SelectOption[] = [
    { value: 'desc', label: 'Más recientes primero' },
    { value: 'asc', label: 'Más antiguos primero' },
  ];

  readonly priorityOptions: SelectOption[] = [
    { value: 'Low', label: 'Baja' },
    { value: 'Medium', label: 'Media' },
    { value: 'High', label: 'Alta' },
  ];

  constructor() {
    this.events.taskCreated$.pipe(takeUntilDestroyed()).subscribe(() => this.loadTasks());
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadTasks();
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => this.users.set(users),
      error: (error: unknown) => this.errorMessage.set(extractErrorMessage(error)),
    });
  }

  loadTasks(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.taskService
      .getAll({
        userId: this.userFilter() === '' ? null : Number(this.userFilter()),
        status: null,
        sort: this.sortOrder(),
      })
      .subscribe({
        next: (tasks) => {
          this.tasks.set(tasks);
          this.loading.set(false);
        },
        error: (error: unknown) => {
          this.errorMessage.set(extractErrorMessage(error));
          this.loading.set(false);
        },
      });
  }

  onDrop(event: CdkDragDrop<TaskItem[]>, targetStatus: TaskStatus): void {
    const task = event.item.data as TaskItem;
    if (!task || task.status === targetStatus) {
      return;
    }

    if (task.status === 'Pending' && targetStatus === 'Done') {
      this.errorMessage.set('No se permite cambiar una tarea directamente de Pending a Done.');
      return;
    }

    this.observationText.set('');
    this.observationModal.set({ task, targetStatus });
  }

  confirmStatusChange(): void {
    const modal = this.observationModal();
    if (!modal) return;
    if (!this.observationText().trim()) return;

    const request: UpdateTaskStatusRequest = {
      status: modal.targetStatus,
      observation: this.observationText().trim(),
    };

    this.observationModal.set(null);

    this.taskService.updateStatus(modal.task.id, request).subscribe({
      next: (updatedTask) => {
        this.tasks.update((tasks) =>
          tasks.map((current) => (current.id === updatedTask.id ? updatedTask : current)),
        );
        this.successMessage.set(
          `Estado de la tarea "${updatedTask.title}" actualizado a "${updatedTask.status}".`,
        );
      },
      error: (error: unknown) => this.errorMessage.set(extractErrorMessage(error)),
    });
  }

  cancelStatusChange(): void {
    this.observationModal.set(null);
    this.observationText.set('');
    this.loadTasks();
  }

  openEdit(task: TaskItem): void {
    const priority = (task.metadata?.['priority'] as string) ?? 'High';
    this.editModal.set({
      task,
      title: task.title,
      description: task.description,
      assignedUserId: task.assignedUserId,
      priority,
      history: [],
      activities: [],
      loading: true,
      newActivity: '',
      saving: false,
    });

    // Load both bitacoras.
    this.taskService.getHistory(task.id).subscribe({
      next: (entries) => {
        const current = this.editModal();
        if (current) {
          this.editModal.set({ ...current, history: entries });
        }
      },
      error: (error: unknown) => this.errorMessage.set(extractErrorMessage(error)),
    });

    this.taskService.getActivities(task.id).subscribe({
      next: (entries) => {
        const current = this.editModal();
        if (current) {
          this.editModal.set({ ...current, activities: entries, loading: false });
        }
      },
      error: (error: unknown) => {
        this.errorMessage.set(extractErrorMessage(error));
        const current = this.editModal();
        if (current) {
          this.editModal.set({ ...current, loading: false });
        }
      },
    });
  }

  closeEdit(): void {
    this.editModal.set(null);
  }

  saveEdit(): void {
    const modal = this.editModal();
    if (!modal || !modal.title.trim() || !modal.description.trim()) return;

    const metadata: Record<string, unknown> = { ...(modal.task.metadata ?? {}) };
    metadata['priority'] = modal.priority;

    const request = {
      title: modal.title.trim(),
      description: modal.description.trim(),
      assignedUserId: modal.task.assignedUserId,
      metadata,
    };

    this.editModal.set({ ...modal, saving: true });

    this.taskService.updateTask(modal.task.id, request).subscribe({
      next: (updatedTask) => {
        this.tasks.update((tasks) =>
          tasks.map((current) => (current.id === updatedTask.id ? updatedTask : current)),
        );
        this.editModal.set(null);
        this.successMessage.set(`Tarea "${updatedTask.title}" actualizada.`);
      },
      error: (error: unknown) => {
        this.errorMessage.set(extractErrorMessage(error));
        const current = this.editModal();
        if (current) {
          this.editModal.set({ ...current, saving: false });
        }
      },
    });
  }

  submitActivity(): void {
    const modal = this.editModal();
    if (!modal || !modal.newActivity.trim()) return;
    this.taskService.addActivity(modal.task.id, modal.newActivity.trim()).subscribe({
      next: (entry) => {
        const current = this.editModal();
        if (current) {
          this.editModal.set({
            ...current,
            activities: [...current.activities, entry],
            newActivity: '',
          });
        }
        this.successMessage.set('Actividad registrada.');
      },
      error: (error: unknown) => this.errorMessage.set(extractErrorMessage(error)),
    });
  }

  priorityOf(task: TaskItem): string | null {
    const priority = task.metadata?.['priority'];
    return typeof priority === 'string' ? priority : null;
  }

  priorityLabel(priority: string): string {
    return TaskListComponent.priorityLabels[priority] ?? priority;
  }

  toString(value: unknown): string {
    return String(value);
  }

  toNumber(value: unknown): number {
    return Number(value);
  }
}
