import { Component, Inject, Injectable } from '@angular/core';
import { FormGroup, FormControl } from '@angular/Forms';
import { CreateService } from '../services/createService'

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls:['./create-account.component.css']
})
export class CreateAccountComponent {

  constructor(private createService: CreateService ) {}
   createForm = new FormGroup( {
    email: new FormControl(' '),
    password: new FormControl(' ')
})

  success;
  new_account: Account;
  onSubmit() {
  this.createService.createAccount(this.createForm.get('email').value, this.createForm.get('password').value)
  }
}




