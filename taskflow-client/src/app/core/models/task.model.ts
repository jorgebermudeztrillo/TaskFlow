export interface BoardColumn {
  id: number;
  name: string;
  tasks: TaskItem[];
}

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  order: number;
  boardColumnId: number;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  boardColumnId: number;
}

export interface MoveTaskRequest {
  boardColumnId: number;
  order: number;
}
