import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { LoginResponseModel } from '../models/LoginResponseModel';
import { FormGroup } from '@angular/forms';
import { catchError, map } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { BotsResponseModel } from '../models/BotsResponseModel';

@Injectable()
export class ProfileService {

  constructor(private http: HttpClient) { }

  getBots(): Observable<BotsResponseModel[]>{
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.get<BotsResponseModel[]>('/api/Bot/GetBotsByUser', httpOptions).pipe(map((res) => {
      return res;
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }

  createBot(form: FormGroup): Observable<any> {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.post<any>('/api/Bot/CreateBot', form.value, httpOptions).pipe(map((res) => {
      return res;
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }

  deleteBot(id: number): Observable<any> {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.post<any>('/api/Bot/DeleteBot', id, httpOptions).pipe(map((res) => {
      return res;
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }

  errorHandler(error: HttpErrorResponse, caught: Observable<any>) {
    if (error.status == 401) {
      localStorage.removeItem('auth_token');
      return throwError(error);
    }
    console.log('error caught: ', error);
    if (error.error != null && (error.error.status == "INVALID_TOKEN" || error.error.status == "MAX_TOKEN_ISSUE_REACHED")) {
      console.log('token has expired');
    }
    return throwError(error);
  }

}
