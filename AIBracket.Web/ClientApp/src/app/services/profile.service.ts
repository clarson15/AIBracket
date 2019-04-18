import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { BotsResponseModel } from '../models/BotsResponseModel';
import { BotHistoryResponseModel } from '../models/BotHistoryResponseModel';

@Injectable()
export class ProfileService {

  constructor(private http: HttpClient) { }

  getBots(id: string): Observable<BotsResponseModel[]>{
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.get<BotsResponseModel[]>('/api/Bot/GetBotsByUser?Id=' + id, httpOptions);
  }

  getBotDetails(id: string): Observable<BotsResponseModel> {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    };
    return this.http.get<BotsResponseModel>('/api/Bot/GetBotDetails?Id=' + id, httpOptions);
  }

  getBotHistory(id: string): Observable<BotHistoryResponseModel[]> {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.get<BotHistoryResponseModel[]>('/api/Bot/GetBotHistory?Id=' + id, httpOptions);
  }

  createBot(form: FormGroup): Observable<string> {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.post<string>('/api/Bot/CreateBot', form.value, httpOptions);
  }

  deleteBot(id: string): Observable<any> {
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
      })
    };
    return this.http.post<any>('/api/Bot/DeleteBot', "\"" + id + "\"", httpOptions);
  }

}
