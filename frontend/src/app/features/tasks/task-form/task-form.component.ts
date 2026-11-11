import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../../core/models/user.model';
import { AppEventsService } from '../../../core/services/app-events.service';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { AlertComponent } from '../../../shared/components/alert/alert.component';
import { SelectComponent, SelectOption } from '../../../shared/components/select/select.component';

// 10-11-2026 Construido por: Ing GAAE - Reactive form to create tasks assigned to a user.
@Component({
  selector: 'app-task-form',
  imports: [ReactiveFormsModule, SelectComponent, AlertComponent],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css',
})
export class TaskFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly userService = inject(UserService);
  private readonly events = inject(AppEventsService);

  readonly users = signal<User[]>([]);
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly userOptions = computed<SelectOption[]>(() =>
    this.users().map((user) => ({ value: user.id, label: `${user.name} (${user.email})` })),
  );

  readonly priorityOptions: SelectOption[] = [
    { value: 'Low', label: 'Baja' },
    { value: 'Medium', label: 'Media' },
    { value: 'High', label: 'Alta' },
  ];

  readonly form = this.formBuilder.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    assignedUserId: [null as number | null, [Validators.required, Validators.min(1)]],
    priority: ['High'],
  });

  constructor() {
    // Reload the user list whenever a new user is created in the sibling form.
    this.events.userCreated$.pipe(takeUntilDestroyed()).subscribe(() => this.loadUsers());
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => this.users.set(users),
      error: (error: unknown) => this.errorMessage.set(this.extractErrorMessage(error)),
    });
  }

  submit(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitting.set(true);

    this.taskService
      .create({
        title: value.title ?? '',
        description: value.description ? value.description : null,
        assignedUserId: Number(value.assignedUserId),
        metadata: { priority: value.priority ?? 'High' },
      })
      .subscribe({
        next: (task) => {
          this.successMessage.set(`Tarea "${task.title}" creada correctamente.`);
          this.form.reset({ title: '', description: '', assignedUserId: null, priority: 'High' });
          this.submitting.set(false);
          this.events.notifyTaskCreated();
        },
        error: (error: unknown) => {
          this.errorMessage.set(this.extractErrorMessage(error));
          this.submitting.set(false);
        },
      });
  }

  private extractErrorMessage(error: unknown): string {
    const httpError = error as { error?: { detail?: string; title?: string }; message?: string };
    return httpError?.error?.detail ?? httpError?.error?.title ?? httpError?.message ?? 'Ocurrió un error inesperado.';
  }
}
