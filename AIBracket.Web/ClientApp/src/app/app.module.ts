import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router'
import { MatCardModule, MatToolbarModule, MatExpansionModule, MatButtonModule, MatInputModule, MatFormFieldModule, MatIconModule, MatMenuModule, MatDividerModule, MatSelectModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component'
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { CreateAccountComponent } from './create-account/create-account.component';
import { ProfileComponent } from './profile/profile.component';

import { AccountService } from './services/account.service';
import { ProfileService } from './services/profile.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    CreateAccountComponent,
    LoginComponent,
    ProfileComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    //Forms
    ReactiveFormsModule,
    //Material
    MatToolbarModule,
    MatExpansionModule,
    MatCardModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    MatSelectModule,
    //Routes
    RouterModule.forRoot([
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent, pathMatch: 'full' },
      { path: 'create-account', component: CreateAccountComponent },
      { path: 'login', component: LoginComponent },
      { path: 'profile', component: ProfileComponent }
    ])
  ],
  providers: [AccountService, ProfileService],
  bootstrap: [AppComponent]
})
export class AppModule { }
