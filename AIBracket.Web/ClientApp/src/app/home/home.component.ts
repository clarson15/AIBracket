import { Component, ViewChild, ElementRef, OnInit, NgZone, Input } from '@angular/core';
import { GameService } from '../services/game.service';
import { AccountService } from '../services/account.service';
import { ProfileResponseModel } from '../models/ProfileResponseModel'; 
import { MatTableDataSource } from '@angular/material';
import * as moment from 'moment';

export interface LeaderboardEntry {

  position: number;
  name: string;
  score: number;
  startDate: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [GameService]
})

export class HomeComponent implements OnInit {

  ActiveGame: boolean = false;
  SpectatorId: string;
  ActiveGameId: string;
  public user: ProfileResponseModel;
  public leaderboard: MatTableDataSource<LeaderboardEntry>;
  displayedColumn = ['position', 'name', 'score', 'date'];

  constructor(private gameservice: GameService, private accountservice: AccountService) { }

  ngOnInit() {
    this.gameservice.getFeaturedGame().subscribe(
      data => {
        if (data == '0') {
          this.ActiveGame = false;
        }
        else {
          this.accountservice.getSpectatorId().subscribe(
            data2 => {
              this.ActiveGameId = data;
              this.SpectatorId = data2.spectatorId;
              this.ActiveGame = true;
            },
            err => {
            });
        }
      },
      err => {
        console.log(err.error);
          });
      this.gameservice.getPacmanLeaderboard().subscribe(
        data => {
          var counter = 1;
          data.forEach(obj => {
            obj.position = counter;
            obj.startDate = moment.utc(obj.startDate).fromNow();
            counter += 1;
          });
          this.leaderboard = new MatTableDataSource<LeaderboardEntry>(data);
          },
          err => {
              console.log(err.error);
          });
  }

  isLoggedIn() {
    return localStorage.getItem('auth_token') == null;
  }
}

