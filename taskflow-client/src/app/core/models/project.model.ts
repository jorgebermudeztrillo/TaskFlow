export interface Project {
  id: number;
  name: string;
  description: string;
  createdAt: string;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
}
