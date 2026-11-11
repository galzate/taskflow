import { Component, effect, input, output, signal } from '@angular/core';

// 10-11-2026 Construido por: Ing GAAE - Alert that hides itself after the configured duration.
@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrl: './alert.component.css',
})
export class AlertComponent {
  private static readonly fadeOutMs = 200;

  readonly message = input<string | null>(null);
  readonly type = input<'success' | 'error'>('success');
  readonly duration = input(5000);
  readonly dismissed = output<void>();

  readonly leaving = signal(false);

  constructor() {
    effect((onCleanup) => {
      if (!this.message()) {
        this.leaving.set(false);
        return;
      }

      this.leaving.set(false);
      const fadeTimer = setTimeout(() => this.leaving.set(true), this.duration());
      const dismissTimer = setTimeout(
        () => this.dismissed.emit(),
        this.duration() + AlertComponent.fadeOutMs,
      );

      onCleanup(() => {
        clearTimeout(fadeTimer);
        clearTimeout(dismissTimer);
      });
    });
  }
}
