import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AppEventsService } from '../../../core/services/app-events.service';
import { UserService } from '../../../core/services/user.service';
import { extractErrorMessage } from '../../../core/utils/error-message';
import { AlertComponent } from '../../../shared/components/alert/alert.component';

// 10-11-2026 Construido por: Ing GAAE - Reactive form to create users.
@Component({
  selector: 'app-user-form',
  imports: [ReactiveFormsModule, AlertComponent],
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
        this.successMessage.set(`Usuario "${user.name}" creado correctamente.`);
        this.form.reset({ name: '', email: '' });
        this.submitting.set(false);
        this.events.notifyUserCreated();
      },
      error: (error: unknown) => {
        this.errorMessage.set(extractErrorMessage(error));
        this.submitting.set(false);
      },
    });
  }
}
