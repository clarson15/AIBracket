import { Component, OnInit } from '@angular/core';
import { AccountService } from './services/account.service';
import { ProfileResponseModel } from './models/ProfileResponseModel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  account: ProfileResponseModel;
  sideNavToggled: boolean = false;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    if (localStorage.getItem('auth_token') != null) {
      this.attemptLogin();
    }
  }

  attemptLogin() {
    this.accountService.getProfile().subscribe(
      data => {
        this.account = data;
      },
      err => {
        console.log(err);
      });
  }

  onLogout() {
    this.account = null;
    localStorage.removeItem('auth_token');
  }

  onActivate($event) {
    if (this.account == null && localStorage.getItem('auth_token') != null) {
      this.attemptLogin();
    }
  }

  toggleNav($event) {
    this.sideNavToggled = !this.sideNavToggled;
  }
}
