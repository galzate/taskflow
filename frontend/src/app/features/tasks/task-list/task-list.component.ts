import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TaskItem, TaskStatus } from '../../../core/models/task.model';
import { User } from '../../../core/models/user.model';
import { AppEventsService } from '../../../core/services/app-events.service';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';

// 10-11-2026 Construido por: Ing GAAE - Task list with status/user filters and status transitions.
@Component({
  selector: 'app-task-list',
  imports: [FormsModule, DatePipe],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent implements OnInit {
  private static readonly statusLabels: Record<TaskStatus, string> = {
    Pending: 'Pendiente',
    InProgress: 'En progreso',
    Done: 'Completada',
  };

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

  readonly counts = computed(() => {
    const tasks = this.tasks();
    return {
      total: tasks.length,
      pending: tasks.filter((task) => task.status === 'Pending').length,
      inProgress: tasks.filter((task) => task.status === 'InProgress').length,
      done: tasks.filter((task) => task.status === 'Done').length,
    };
  });

  readonly statuses: TaskStatus[] = ['Pending', 'InProgress', 'Done'];

  statusFilter: TaskStatus | '' = '';
  userFilter: number | '' = '';
  sortOrder: 'asc' | 'desc' = 'desc';

  constructor() {
    // Refresh the list when a task or a user is created in the sibling forms.
    this.events.taskCreated$.pipe(takeUntilDestroyed()).subscribe(() => this.loadTasks());
    this.events.userCreated$.pipe(takeUntilDestroyed()).subscribe(() => this.loadUsers());
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadTasks();
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => this.users.set(users),
      error: (error: unknown) => this.errorMessage.set(this.extractErrorMessage(error)),
    });
  }

  loadTasks(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.taskService
      .getAll({
        userId: this.userFilter === '' ? null : Number(this.userFilter),
        status: this.statusFilter === '' ? null : this.statusFilter,
        sort: this.sortOrder,
      })
      .subscribe({
        next: (tasks) => {
          this.tasks.set(tasks);
          this.loading.set(false);
        },
        error: (error: unknown) => {
          this.errorMessage.set(this.extractErrorMessage(error));
          this.loading.set(false);
        },
      });
  }

  changeStatus(task: TaskItem, status: TaskStatus): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.taskService.updateStatus(task.id, status).subscribe({
      next: (updatedTask) => {
        this.tasks.update((tasks) =>
          tasks.map((current) => (current.id === updatedTask.id ? updatedTask : current)),
        );
        this.successMessage.set(
          `Estado de la tarea "${updatedTask.title}" actualizado a "${this.statusLabel(updatedTask.status)}".`,
        );
      },
      error: (error: unknown) => this.errorMessage.set(this.extractErrorMessage(error)),
    });
  }

  isTransitionDisabled(task: TaskItem, status: TaskStatus): boolean {
    if (task.status === status) {
      return true;
    }

    // Business rule: Pending cannot jump directly to Done, and Done cannot go back to Pending.
    if (task.status === 'Pending' && status === 'Done') {
      return true;
    }
    if (task.status === 'Done' && status === 'Pending') {
      return true;
    }

    return false;
  }

  transitionHint(task: TaskItem, status: TaskStatus): string {
    if (task.status === status) {
      return 'Este es el estado actual.';
    }
    if (task.status === 'Pending' && status === 'Done') {
      return 'No permitido: primero mueve la tarea a En progreso.';
    }
    if (task.status === 'Done' && status === 'Pending') {
      return 'No permitido: reabre la tarea pasando por En progreso.';
    }
    return `Cambiar estado a ${this.statusLabel(status)}.`;
  }

  statusLabel(status: TaskStatus): string {
    return TaskListComponent.statusLabels[status];
  }

  priorityOf(task: TaskItem): string | null {
    const priority = task.metadata?.['priority'];
    return typeof priority === 'string' ? priority : null;
  }

  priorityLabel(priority: string): string {
    return TaskListComponent.priorityLabels[priority] ?? priority;
  }

  private extractErrorMessage(error: unknown): string {
    const httpError = error as { error?: { detail?: string; title?: string }; message?: string };
    return httpError?.error?.detail ?? httpError?.error?.title ?? httpError?.message ?? 'Ocurrió un error inesperado.';
  }
}
