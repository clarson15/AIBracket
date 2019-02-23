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
      this.createForm.get('Password').setErrors({ password: "password mismatch" });
      this.createForm.get('Confirm').setErrors({ password: "password mismatch" });
    }
    else {
      this.createForm.get('Confirm').disable();
      this.createService.createAccount(this.createForm).subscribe(
        data => {
          this.router.navigate(['/home']);
        },
        err => {
          console.log(err.error);
          if (err.error.DuplicateUserName) {
            this.createForm.get('UserName').setErrors({ username: 'duplicate username' });
          }
        });
      this.createForm.get('Confirm').enable();
    }
  }
}




