import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AppEventsService } from '../../../core/services/app-events.service';
import { UserService } from '../../../core/services/user.service';

// 10-11-2026 Construido por: Ing GAAE - Reactive form to create users.
@Component({
  selector: 'app-user-form',
  imports: [ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.css',
})
export class UserFormComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly events = inject(AppEventsService);

  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
  });

  submit(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitting.set(true);

    this.userService.create({ name: value.name ?? '', email: value.email ?? '' }).subscribe({
      next: (user) => {
        this.successMessage.set(`User "${user.name}" created successfully.`);
        this.form.reset({ name: '', email: '' });
        this.submitting.set(false);
        this.events.notifyUserCreated();
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.extractErrorMessage(error));
        this.submitting.set(false);
      },
    });
  }

  private extractErrorMessage(error: unknown): string {
    const httpError = error as { error?: { detail?: string; title?: string }; message?: string };
    return httpError?.error?.detail ?? httpError?.error?.title ?? httpError?.message ?? 'Unexpected error.';
  }
}
