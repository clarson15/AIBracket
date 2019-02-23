import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router'
import { MatCardModule, MatToolbarModule, MatExpansionModule, MatButtonModule, MatInputModule, MatFormFieldModule, MatFormField} from '@angular/material'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component'
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { CreateAccountComponent } from './create-account/create-account.component';

import { AccountService } from './services/account.service';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    CreateAccountComponent,
    LoginComponent
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
    //Routes
    RouterModule.forRoot([
      { path: 'home', component: HomeComponent, pathMatch: 'full' },
      { path: 'create-account', component: CreateAccountComponent },
      { path: 'login', component: LoginComponent },
    ])
  ],
  providers: [AccountService],
  bootstrap: [AppComponent]
})
export class AppModule { }
