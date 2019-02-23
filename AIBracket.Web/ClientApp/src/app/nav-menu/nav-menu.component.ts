import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileResponseModel } from '../models/ProfileResponseModel';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  @Input('Account')
  account: ProfileResponseModel;

  constructor(private router: Router) { }

  collapse() {
    this.isExpanded = false;
  }

  logout() {
    localStorage.removeItem('auth_token');
    this.account = null;
    this.router.navigate(['/login']);
  }

  isLoggedIn() {
    return localStorage.getItem('auth_token') != null;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
