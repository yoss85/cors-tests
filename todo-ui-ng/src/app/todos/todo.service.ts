import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Todo } from "../data/todo";


@Injectable()
export class TodoService {
    todoUrl = 'http://localhost:7071/api/todo'; 

    constructor(private http: HttpClient) {
    }

    getAllTodos(): Observable<Todo[]> {
        return this.http.get<Todo[]>(this.todoUrl);
    }

    createTodo(todo: Todo): Observable<Todo> {
        console.log('Created new todo', todo);
        return this.http.post<Todo>(this.todoUrl, todo);
    }
}
