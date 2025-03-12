import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { PersonViewModel } from '../models/person-view-model';


@Injectable({
  providedIn: 'root'
})
export class PersonService {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getById(id: number): Observable<PersonViewModel> {
    console.log('getById called ' + this.baseUrl + `api/person/${id}`);
    return this.http.get<PersonViewModel>(this.baseUrl + `api/person/${id}`);    
  }

  getPeople(): Observable<PersonViewModel[]> {
    console.log('getPeople called ' + this.baseUrl + 'api/person');
    return this.http.get<PersonViewModel[]>(this.baseUrl + 'api/person');
  }

  createPerson(person: PersonViewModel): Observable<PersonViewModel> {
    console.log('createPerson called ' + this.baseUrl + 'api/person');
    return this.http.post<PersonViewModel>(this.baseUrl + 'api/person', person);
  }

  updatePerson(person: PersonViewModel): Observable<PersonViewModel> {
    console.log('updatePerson called ' + this.baseUrl + 'api/person');
    return this.http.put<PersonViewModel>(this.baseUrl + 'api/person', person);
     
  }

}
