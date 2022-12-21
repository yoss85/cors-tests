import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Todo } from 'src/app/data/todo';
import { TodoService } from '../todo.service';

@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css']
})
export class TodoListComponent implements OnInit{
  todos$: Observable<Todo[]>;
  
  constructor(private todoService: TodoService) {
    this.todos$ = this.todoService.getAllTodos();
  }
  ngOnInit(): void {
    this.todos$ = this.todoService.getAllTodos();
  }

}
