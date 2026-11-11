// 10-11-2026 Construido por: Ing GAAE - Task models shared with the API contract.
export type TaskStatus = 'Pending' | 'InProgress' | 'Done';

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  status: TaskStatus;
  assignedUserId: number;
  assignedUserName: string;
  metadata: Record<string, unknown> | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  assignedUserId: number;
  metadata?: Record<string, unknown> | null;
}

export interface UpdateTaskStatusRequest {
  status: TaskStatus;
}
