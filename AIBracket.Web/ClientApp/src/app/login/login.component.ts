import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { LoginService } from '../services/loginService';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  constructor(private loginService: LoginService) { }
  profileForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  })

  onSubmit() {
    this.loginService.login(this.profileForm.get('email').value, this.profileForm.get('password').value)
  }
}
