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
        this.loginService.getProfile(data.id).subscribe(profile => {
          this.loginService.setUser(profile);
          localStorage.setItem('auth_token', data.auth_token);
        }, err => {
          console.log('error getting profile for user.');
        });
        this.router.navigate(['/home']);
      },
      err => {
        if (err.error.login_failure) {
          this.profileForm.get('Password').setErrors({ password: 'incorrect password' });
        }
      });
    }
}
