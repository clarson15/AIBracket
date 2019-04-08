import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';

@Injectable()
export class GameService {

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) { }

  getFeaturedGame(): Observable<any> {
    let headers = new HttpHeaders({
      'Content-Type': 'text/plain'
    });
    return this.http.get('/api/Game/GetFeaturedGame', { responseType: 'text' }).pipe(map((res) => {
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

  getPacmanLeaderboard(): Observable<any> {
    let headers = new HttpHeaders({
      'Content-Type': 'text/plain'
    });
    return this.http.get<any>('/api/Game/GetLeaderboard').pipe(map((res) => {
      return res;
    }), catchError((err, obs) => this.errorHandler(err, obs)));
  }
}
