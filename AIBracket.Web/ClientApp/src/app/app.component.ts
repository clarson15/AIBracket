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

  onLogout() {
    this.account = null;
    localStorage.removeItem('auth_token');
  }

  onActivate($event) {
    console.log($event);
    console.log(this.account);
    console.log(localStorage.getItem('auth_token'));
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
