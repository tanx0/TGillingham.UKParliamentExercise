import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { DepartmentViewModel } from '../models/department-view-model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getDepartments(): Observable<DepartmentViewModel[]> {
    console.log('getPeople called ' + this.baseUrl + 'api/department');
    return this.http.get<DepartmentViewModel[]>(this.baseUrl + 'api/department');    
  }
}
