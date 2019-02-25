import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { ProfileService } from '../services/profile.service';
import { BotsResponseModel } from '../models/BotsResponseModel';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  constructor(private loginService: AccountService, private profileService: ProfileService, private router: Router) { }

  private bots: BotsResponseModel[];
  private CreationExpanded: boolean;

  newBotForm = new FormGroup({
    Name: new FormControl(''),
    Game: new FormControl('')
  })

  updateBots() {
    this.CreationExpanded = false;
    this.profileService.getBots().subscribe(
      data => {
        this.bots = data;
      },
      err => {
        console.log(err);
      });
  }

  createBot() {
    this.profileService.createBot(this.newBotForm).subscribe(data => {
      this.newBotForm.reset();
      this.updateBots();
    }, err => {
      console.log(err);
    });
  }

  mapGameName(game: number) {
    switch (game) {
      case 1:
        return 'Pacman';
      case 2:
        return 'Tanks';
      default:
        return 'UNKNOWN';
    }
  }

  deleteBot(id: number) {
    console.log(id);
    this.profileService.deleteBot(id).subscribe(data => {
      this.updateBots();
    }, err => {
    });
  }

  ngOnInit() {
    this.CreationExpanded = false;
    if (localStorage.getItem('auth_token') == null) {
      this.router.navigate['/login'];
    }
    this.updateBots();
  }
}
