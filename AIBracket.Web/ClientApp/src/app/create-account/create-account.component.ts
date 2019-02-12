import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormGroup, FormControl } from '@angular/Forms';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls:['./create-account.component.css']
})
export class CreateAccountComponent {
  constructor(private http: HttpClient) {

  }
   createForm = new FormGroup( {
    email: new FormControl(' '),
    password: new FormControl(' ')
})

  success;
  new_account:Account;
  createAccount() {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    console.log(this.createForm.get('email').value, " ", this.createForm.get('password').value);
    this.http.post('/api/SampleData/Register', { username: this.createForm.get('email').value, password: this.createForm.get('password').value }, httpOptions).subscribe(result => {
      console.log(result + "<-- RESULT!!! \n");
    }, error => console.error(error));
  }
}




