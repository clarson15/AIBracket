import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private loginService: AccountService, private router: Router) { }

  ngOnInit() {
    if (localStorage.getItem('auth_token')) {
      this.router.navigate(['/home']);
    }
  }

  profileForm = new FormGroup({
    UserName: new FormControl(''),
    Password: new FormControl(''),
  });

  onSubmit() {
    this.loginService.login(this.profileForm).subscribe(
      data => {
        localStorage.setItem('auth_token', data.auth_token);
        this.router.navigate(['/home']);
      },
      err => {
        console.log(err.error);
        if (err.error.login_failure) {
          this.profileForm.get('Password').setErrors({ password: 'incorrect password' });
        }
      });
    }
}
