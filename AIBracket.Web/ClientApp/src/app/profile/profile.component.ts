import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { ProfileService } from '../services/profile.service';
import { BotsResponseModel } from '../models/BotsResponseModel';
import { ProfileResponseModel } from '../models/ProfileResponseModel';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  constructor(private profileService: ProfileService, private router: Router) { }

  private bots: BotsResponseModel[];
  private CreationExpanded: boolean;
  private newId: string;

  public user: ProfileResponseModel;

  newBotForm = new FormGroup({
    Name: new FormControl(''),
    Game: new FormControl('')
  })

  updateBots() {
    this.CreationExpanded = false;
    this.profileService.getBots().subscribe(
      data => {
        this.bots = data;
        data.forEach(x => {
          x.showSecret = false;
          this.profileService.getBotHistory(x.id).subscribe(history => {
            console.log(history);
            this.bots.find(b => b.id == x.id).history = history;
          })
        });
        if (this.bots.length == 0) {
          this.CreationExpanded = true;
        }
      },
      err => {
        console.log(err);
      });
  }

  showSecret(i: number) {
    this.bots[i].showSecret = true;
  }

  createBot() {
    this.profileService.createBot(this.newBotForm).subscribe(data => {
      this.newId = data;
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

  deleteBot(id: string) {
    console.log(id);
    this.profileService.deleteBot(id).subscribe(data => {
      this.updateBots();
    }, err => {
      console.log(err);
    });
  }

  ngOnInit() {
    this.CreationExpanded = false;
    this.updateBots();
  }
}
