import { Component } from '@angular/core';
import { Todo } from 'src/app/data/todo';
import { TodoService } from '../todo.service';

@Component({
  selector: 'app-todo-edit',
  templateUrl: './todo-edit.component.html',
  styleUrls: ['./todo-edit.component.css']
})
export class TodoEditComponent {
  submitted = false;
  model!: Todo;

  constructor(private todoService: TodoService) {}

  onSubmit() {
    let todo = {
      id: 0,
      name: 'name',
      description: 'description',
      completed: false,
    } as Todo;
    this.todoService.createTodo(todo).subscribe();
  }
}
