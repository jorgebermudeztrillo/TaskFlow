import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BoardColumn, CreateTaskRequest, MoveTaskRequest, TaskItem } from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly apiUrl = 'http://localhost:5188/api/task';

  constructor(private http: HttpClient) { }

  getBoard(projectId: number): Observable<BoardColumn[]> {
    return this.http.get<BoardColumn[]>(`${this.apiUrl}/board/${projectId}`);
  }

  create(data: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.apiUrl, data);
  }

  move(taskId: number, data: MoveTaskRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${taskId}/move`, data);
  }

  delete(taskId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${taskId}`);
  }
}
