import { Component } from '@angular/core';
import { TaskFormComponent } from './features/tasks/task-form/task-form.component';
import { TaskListComponent } from './features/tasks/task-list/task-list.component';
import { UserFormComponent } from './features/users/user-form/user-form.component';

// 10-11-2026 Construido por: Ing GAAE - Root component that composes the task management screen.
@Component({
  selector: 'app-root',
  imports: [TaskFormComponent, TaskListComponent, UserFormComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}
