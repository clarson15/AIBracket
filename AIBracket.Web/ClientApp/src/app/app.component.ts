import { Component, OnInit } from '@angular/core';
import { AccountService } from './services/account.service';
import { ProfileResponseModel } from './models/ProfileResponseModel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [AccountService]
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
    this.accountService.currentAccount.subscribe(acc => {
      this.account = acc;
    });
    if (localStorage.getItem('auth_token') != null) {
      this.attemptLogin();
    }
    else {
      this.isReady = true;
    }
  }

  attemptLogin() {
    this.accountService.getProfile('').subscribe(
      data => {
        this.accountService.setUser(data);
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

  onActivate($event) {
  }

  toggleNav($event) {
    this.sideNavToggled = !this.sideNavToggled;
  }
}
