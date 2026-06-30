import { Component, OnInit, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ProjectService } from '../../../core/services/project.service';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { Project } from '../../../core/models/project.model';
import { BoardColumn } from '../../../core/models/task.model';
import { Router } from '@angular/router';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatIconModule, DragDropModule],
  templateUrl: './board.html',
  styleUrl: './board.scss',
})
export class Board implements OnInit {
  projects = signal<Project[]>([]);
  selectedProject = signal<Project | null>(null);
  columns = signal<BoardColumn[]>([]);
  dropListIds = computed(() => this.columns().map(c => 'column-' + c.id));

  newProjectName = '';
  newProjectDescription = '';
  showNewProjectForm = false;

  newTaskTitle = '';
  newTaskDescription = '';
  addingTaskToColumn: number | null = null;

  openTaskForm(columnId: number): void {
    this.addingTaskToColumn = columnId;
    this.newTaskTitle = '';
    this.newTaskDescription = '';
  }

  cancelTaskForm(): void {
    this.addingTaskToColumn = null;
  }

  createTask(columnId: number): void {
    if (!this.newTaskTitle.trim()) return;

    this.taskService.create({
      title: this.newTaskTitle,
      description: this.newTaskDescription,
      boardColumnId: columnId
    }).subscribe({
      next: (task) => {
        this.columns.update(cols =>
          cols.map(col =>
            col.id === columnId
              ? { ...col, tasks: [...col.tasks, task] }
              : col
          )
        );
        this.addingTaskToColumn = null;
      }
    });
  }

  constructor(
    private projectService: ProjectService,
    private taskService: TaskService,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.projectService.getAll().subscribe({
      next: (projects) => this.projects.set(projects)
    });
  }

  selectProject(project: Project): void {
    this.selectedProject.set(project);
    this.loadBoard(project.id);
  }

  loadBoard(projectId: number): void {
    this.taskService.getBoard(projectId).subscribe({
      next: (columns) => this.columns.set(columns)
    });
  }

  onTaskDrop(event: CdkDragDrop<any>, targetColumnId: number): void {
    const cols = this.columns();
    const sourceColIndex = cols.findIndex(c => c.tasks === event.previousContainer.data);
    const targetColIndex = cols.findIndex(c => c.id === targetColumnId);

    if (sourceColIndex === targetColIndex) {
      moveItemInArray(cols[sourceColIndex].tasks, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        cols[sourceColIndex].tasks,
        cols[targetColIndex].tasks,
        event.previousIndex,
        event.currentIndex
      );
    }

    this.columns.set([...cols]);

    const movedTask = cols[targetColIndex].tasks[event.currentIndex];
    this.taskService.move(movedTask.id, {
      boardColumnId: targetColumnId,
      order: event.currentIndex
    }).subscribe();
  }

  createProject(): void {
    if (!this.newProjectName.trim()) return;

    this.projectService.create({
      name: this.newProjectName,
      description: this.newProjectDescription
    }).subscribe({
      next: (project) => {
        this.projects.update(list => [...list, project]);
        this.newProjectName = '';
        this.newProjectDescription = '';
        this.showNewProjectForm = false;
        this.selectProject(project);
      }
    });
  }

  backToProjects(): void {
    this.selectedProject.set(null);
    this.columns.set([]);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
