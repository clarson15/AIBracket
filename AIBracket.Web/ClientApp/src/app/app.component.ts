import { Component } from '@angular/core';
import { AccountService } from './services/account.service';
import { HomeComponent } from './home/home.component';
import { ProfileResponseModel } from './models/ProfileResponseModel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';

  account: ProfileResponseModel;

  constructor(private accountService: AccountService) { }

  onActivate($event) {
    if ($event.constructor.name == "HomeComponent") {
      if (this.account == null && localStorage.getItem('auth_token') != null) {
        this.accountService.getProfile().subscribe(
          data => {
            this.account = data;
          },
          err => {
            console.log(err);
          });
      }
    }
  }
}
