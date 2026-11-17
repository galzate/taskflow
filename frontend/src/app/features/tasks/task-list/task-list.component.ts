// 10-11-2026 Construido por: Ing GAAE - Kanban board with drag and drop status transitions and bitacora.
import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { CdkDrag, CdkDragDrop, CdkDragHandle, CdkDropList } from '@angular/cdk/drag-drop';
import { TaskItem, TaskStatus, TaskStatusHistory, UpdateTaskStatusRequest } from '../../../core/models/task.model';
import { User } from '../../../core/models/user.model';
import { AppEventsService } from '../../../core/services/app-events.service';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { extractErrorMessage } from '../../../core/utils/error-message';
import { AlertComponent } from '../../../shared/components/alert/alert.component';
import { SelectComponent, SelectOption } from '../../../shared/components/select/select.component';

@Component({
  selector: 'app-task-list',
  imports: [FormsModule, DatePipe, CdkDropList, CdkDrag, CdkDragHandle, SelectComponent, AlertComponent],
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

  readonly observationModal = signal<{ task: TaskItem; targetStatus: TaskStatus } | null>(null);
  readonly observationText = signal('');

  readonly historyModal = signal<{ task: TaskItem; entries: TaskStatusHistory[]; loading: boolean } | null>(null);

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

  readonly sortOptions: SelectOption[] = [
    { value: 'desc', label: 'Más recientes primero' },
    { value: 'asc', label: 'Más antiguos primero' },
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

    const request: UpdateTaskStatusRequest = {
      status: modal.targetStatus,
      observation: this.observationText().trim() || undefined,
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
  }

  openHistory(task: TaskItem): void {
    this.historyModal.set({ task, entries: [], loading: true });
    this.taskService.getHistory(task.id).subscribe({
      next: (entries) => {
        const current = this.historyModal();
        if (current) {
          this.historyModal.set({ ...current, entries, loading: false });
        }
      },
      error: (error: unknown) => {
        this.errorMessage.set(extractErrorMessage(error));
        this.historyModal.set(null);
      },
    });
  }

  closeHistory(): void {
    this.historyModal.set(null);
  }

  priorityLabel(priority: string): string {
    return TaskListComponent.priorityLabels[priority] ?? priority;
  }

  priorityOf(task: TaskItem): string | null {
    const priority = task.metadata?.['priority'];
    return typeof priority === 'string' ? priority : null;
  }
}
