import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { BotsResponseModel } from '../models/BotsResponseModel';
import { ProfileService } from '../services/profile.service';
import * as moment from 'moment';

@Component({
  selector: 'bot-details',
  templateUrl: './bot-details.component.html',
  styleUrls: ['./bot-details.component.css'],
  providers: [AccountService, ProfileService]
})
export class BotDetailsComponent implements OnInit {
  
  BotId: string;
  bot: BotsResponseModel;
  SpectatorId: string;
  ready: boolean = false;

  constructor(private router: Router, private route: ActivatedRoute, private accountService: AccountService, private profileService: ProfileService) { }

  ngOnInit() {
    this.route.params.subscribe(data => {
      this.BotId = data['Id'];
      this.profileService.getBotDetails(this.BotId).subscribe(b => {
        this.bot = b;
        this.profileService.getBotHistory(this.BotId).subscribe(history => {
          history.forEach(h => {
            h.endDate = moment.duration(moment(h.endDate).diff(moment(h.startDate))).humanize();
            h.startDate = moment.utc(h.startDate).fromNow();
          });
          this.bot.history = history;
        });
      });
    });
    this.accountService.getSpectatorId().subscribe(data => {
      this.SpectatorId = data.spectatorId;
      this.ready = true;
    });
  }
}
