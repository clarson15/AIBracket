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
  isReady: boolean = false;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    if (document.body.clientWidth >= 1920) {
      this.sideNavToggled = true;
    }
    if (localStorage.getItem('auth_token') != null) {
      this.attemptLogin();
    }
    else {
      this.isReady = true;
    }
  }

  attemptLogin() {
    this.accountService.getProfile().subscribe(
      data => {
        this.account = data;
        if (document.location.pathname == '/profile' && this.account == null) {
          document.location.pathname = 'login';
        }
        this.isReady = true;
      },
      err => {
        console.log(err);
        this.isReady = true;
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
    if ($event.constructor.name == "ProfileComponent") {
      $event.user = this.account;
    }
  }

  toggleNav($event) {
    this.sideNavToggled = !this.sideNavToggled;
  }
}
