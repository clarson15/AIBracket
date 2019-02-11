import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl } from '@angular/Forms';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls:['./create-account.component.css']
})
export class CreateAccountComponent {
  createForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl('')
  })
}


