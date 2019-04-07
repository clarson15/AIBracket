import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class GameService {

  constructor(private http: HttpClient) { }

  getFeaturedGame(): Observable<any> {
    return this.http.get('/api/Game/GetFeaturedGame', { responseType: 'text' });
  }

}
