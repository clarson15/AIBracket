import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls:['./create-account.component.css']
})
export class CreateAccountComponent implements OnInit {

  constructor(private createService: AccountService, private router: Router ) {}
  createForm = new FormGroup({
    UserName: new FormControl(''),
    Password: new FormControl(''),
    Confirm: new FormControl(''),
    FirstName: new FormControl(''),
    LastName: new FormControl('')
  })

  password2: string;

  ngOnInit() {
    if (localStorage.getItem('auth_token') != null) {
      this.router.navigate(['/home']);
    }
  }

  onSubmit() {
    if (this.createForm.get('Confirm').value != this.createForm.get('Password').value) {
      this.createForm.get('Password').setErrors({ mismatch: "password mismatch" });
      this.createForm.get('Confirm').setErrors({ mismatch: "password mismatch" });
    }
    else {
      this.createForm.get('Confirm').disable();
      this.createService.createAccount(this.createForm).subscribe(
        data => {
          let loginForm = new FormGroup({
            UserName: this.createForm.get('UserName'),
            Password: this.createForm.get('Password')
          })
          this.createService.login(loginForm).subscribe(
            data2 => {
              localStorage.setItem('auth_token', data2.auth_token);
              this.router.navigate(['/home']);
            },
            err => {
              console.log(err);
            });
        },
        err => {
          console.log(err.error);
          if (err.error.DuplicateUserName) {
            this.createForm.get('UserName').setErrors({ taken: 'Username already in use.' });
          }
          if (err.error.UsernameLength) {
            this.createForm.get('UserName').setErrors({ length: 'Username must be at least 5 characters.' });
          }
          if (err.error.PasswordTooShort) {
            this.createForm.get('Password').setErrors({ length: "Password must be at least 6 characters." });
            this.createForm.get('Confirm').setErrors({ length: "Password must be at least 6 characters." });
          }
        });
      this.createForm.get('Confirm').enable();
    }
  }
}




