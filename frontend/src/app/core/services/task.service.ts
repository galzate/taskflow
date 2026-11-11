import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { CreateTaskRequest, TaskItem, TaskStatus } from '../models/task.model';

export interface TaskFilters {
  userId?: number | null;
  status?: TaskStatus | null;
  sort?: 'asc' | 'desc';
}

// 10-11-2026 Construido por: Ing GAAE - HTTP client for the tasks endpoints.
@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/tasks`;

  getAll(filters: TaskFilters = {}): Observable<TaskItem[]> {
    let params = new HttpParams();

    if (filters.userId !== undefined && filters.userId !== null) {
      params = params.set('userId', filters.userId);
    }
    if (filters.status) {
      params = params.set('status', filters.status);
    }
    if (filters.sort) {
      params = params.set('sort', filters.sort);
    }

    return this.http.get<TaskItem[]>(this.baseUrl, { params });
  }

  create(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.baseUrl, request);
  }

  updateStatus(id: number, status: TaskStatus): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.baseUrl}/${id}/status`, { status });
  }
}
