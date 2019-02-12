import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  profileForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  })

  createForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl('')
  })
}
