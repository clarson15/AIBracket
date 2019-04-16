import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileResponseModel } from '../models/ProfileResponseModel';
import { isEmpty } from 'rxjs/operators';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  
  account: ProfileResponseModel;
  @Output() logout: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() sidenav: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(private router: Router, private accountService: AccountService) { }

  ngOnInit() {
    this.accountService.currentAccount.subscribe(acc => {
      this.account = acc;
    });
  }

  viewProfile() {
    this.router.navigate(['/profile', this.account.id]);
  }

  getDisplayName() {
    if (this.account.firstName == '' && this.account.lastName == '') {
      return this.account.userName;
    }
    else {
      return this.account.firstName + ' ' + this.account.lastName;
    }
  }

  Logout() {
    this.accountService.setUser(null);
    localStorage.removeItem('auth_token');
    this.router.navigate(['/home']);
  }

  OpenNav() {
    this.sidenav.emit(true);
  }
}
