import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class LoginService {
  constructor(private http: HttpClient) {

  }
  login(email, password) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    console.log(email, password);
    this.http.post('/api/SampleData/Authenticate', { username: email, password: password }, httpOptions).subscribe(ret => {
      if (ret.token) {
        localStorage.setItem('currentUser', JSON.stringify(ret));
      }
    }, error => console.error(error));
  }
}
