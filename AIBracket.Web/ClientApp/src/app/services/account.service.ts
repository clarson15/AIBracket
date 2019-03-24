import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Http, Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { LoginResponseModel } from '../models/LoginResponseModel';
import { FormGroup } from '@angular/forms';
import { catchError, map } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { CreateAccountResponseModel,  } from '../models/CreateAccountResponseModel';
import { ProfileResponseModel } from '../models/ProfileResponseModel';

@Injectable()
export class AccountService {

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) { }

  login(data: FormGroup): Observable<LoginResponseModel>{
    return this.http.post<LoginResponseModel>('/api/Auth/Login', data.value, this.httpOptions).pipe(map((res) => {
      return res;
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }

  createAccount(data: FormGroup): Observable<CreateAccountResponseModel>{
    return this.http.post<CreateAccountResponseModel>('/api/Auth/Register', data.value, this.httpOptions).pipe(map((res) => {
      return res; 
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }

  getProfile(): Observable<ProfileResponseModel> {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
    });
    return this.http.get<ProfileResponseModel>('/api/Auth/GetProfileData', { headers }).pipe(map((res) => {
      return res;
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }

  getSpectatorId(): Observable<any> {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
    });
    return this.http.get<any>('/api/Auth/GetSpectatorId', { headers }).pipe(map((res) => {
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
