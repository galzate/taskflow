import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

// 10-11-2026 Construido por: Ing GAAE - Lightweight event bus used to refresh data between sibling components.
@Injectable({ providedIn: 'root' })
export class AppEventsService {
  private readonly taskCreatedSource = new Subject<void>();
  private readonly userCreatedSource = new Subject<void>();

  readonly taskCreated$ = this.taskCreatedSource.asObservable();
  readonly userCreated$ = this.userCreatedSource.asObservable();

  notifyTaskCreated(): void {
    this.taskCreatedSource.next();
  }

  notifyUserCreated(): void {
    this.userCreatedSource.next();
  }
}
