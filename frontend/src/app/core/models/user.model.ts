// 10-11-2026 Construido por: Ing GAAE - User models shared with the API contract.
export interface User {
  id: number;
  name: string;
  email: string;
  createdAt: string;
}

export interface CreateUserRequest {
  name: string;
  email: string;
}
