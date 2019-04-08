import { Component, ViewChild, ElementRef, OnInit, NgZone } from '@angular/core';
import { GameService } from '../services/game.service';
import { AccountService } from '../services/account.service';

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
  private leaderboard: any;

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
              this.SpectatorId = data2;
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
              this.leaderboard = data;
              console.log(this.leaderboard);
          },
          err => {
              console.log(err.error);
          });
  }
}
