import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router'
import { MatCardModule, MatToolbarModule, MatExpansionModule, MatButtonModule, MatInputModule, MatFormFieldModule, MatIconModule, MatMenuModule, MatDividerModule, MatSelectModule, MatSidenavModule, MatCheckboxModule, MatTab, MatTabsModule, MatListModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { LearnMoreComponent } from './learn-more/learn-more.component';
import { GettingStartedComponent } from './getting-started/getting-started.component';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component'
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CreateAccountComponent } from './create-account/create-account.component';
import { ProfileComponent } from './profile/profile.component';
import { GamePacmanComponent } from './game-pacman/game-pacman.component';

import { AccountService } from './services/account.service';
import { ProfileService } from './services/profile.service';
import { ErrorInterceptor } from './services/error.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CreateAccountComponent,
    LoginComponent,
    ProfileComponent,
    LearnMoreComponent,
    GettingStartedComponent,
    GamePacmanComponent
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
    MatCheckboxModule,
    MatDividerModule,
    MatSelectModule,
    MatSidenavModule,
    MatTabsModule,
    MatListModule,
    //Routes
    RouterModule.forRoot([
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent, pathMatch: 'full' },
      { path: 'create-account', component: CreateAccountComponent },
      { path: 'login', component: LoginComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'learn', component: GettingStartedComponent },
      { path: 'learn-more', component: LearnMoreComponent },
      { path: '**', redirectTo: '/home' }
    ])
  ],
  providers: [AccountService, ProfileService, { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },],
  bootstrap: [AppComponent]
})
export class AppModule { }
