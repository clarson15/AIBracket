import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginResponseModel } from '../models/LoginResponseModel';
import { FormGroup } from '@angular/forms';
import { Observable, BehaviorSubject} from 'rxjs';
import { CreateAccountResponseModel,  } from '../models/CreateAccountResponseModel';
import { ProfileResponseModel } from '../models/ProfileResponseModel';

@Injectable()
export class AccountService {

  private accountSource = new BehaviorSubject<ProfileResponseModel>(null);
  public currentAccount = this.accountSource.asObservable();

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  setUser(profile: ProfileResponseModel) {
    this.accountSource.next(profile);
  }

  constructor(private http: HttpClient) { }

  login(data: FormGroup): Observable<LoginResponseModel>{
    return this.http.post<LoginResponseModel>('/api/Auth/Login', data.value, this.httpOptions);
  }

  createAccount(data: FormGroup): Observable<CreateAccountResponseModel>{
    return this.http.post<CreateAccountResponseModel>('/api/Auth/Register', data.value, this.httpOptions);
  }

  getProfile(id: string): Observable<ProfileResponseModel> {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
    });
    return this.http.get<ProfileResponseModel>('/api/Auth/GetProfileData?Id=' + id, { headers });
  }

  getSpectatorId(): Observable<any> {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
    });
    return this.http.get<any>('/api/Auth/GetSpectatorId', { headers });
  }

}
