import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class CreateService {
  constructor(private http: HttpClient) {

  }
  createAccount(email, password) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    console.log(email, password);
    this.http.post('/api/SampleData/Register', { username: email, password: password }, httpOptions).subscribe(result => {
      console.log(result + "<-- RESULT!!! \n");
    }, error => console.error(error));
  }
}
