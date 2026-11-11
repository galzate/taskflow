import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { CreateUserRequest, User } from '../models/user.model';

// 10-11-2026 Construido por: Ing GAAE - HTTP client for the users endpoints.
@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/users`;

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl);
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.baseUrl, request);
  }
}
