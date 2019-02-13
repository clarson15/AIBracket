import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { HttpHeaders, HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  profileForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  })

  constructor(private http: HttpClient) { }

  login() {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    console.log(this.profileForm.get('email').value, " ", this.profileForm.get('password').value);
    this.http.post('/api/SampleData/Authenticate', { username: this.profileForm.get('email').value, password: this.profileForm.get('password').value }, httpOptions).subscribe(ret => {
      if (ret.token) {
        localStorage.setItem('currentUser', JSON.stringify(ret));
      }
    }, error => console.error(error));
  }
}
