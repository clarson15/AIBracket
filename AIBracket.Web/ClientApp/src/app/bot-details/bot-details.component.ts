import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'bot-details',
  templateUrl: './bot-details.component.html',
  styleUrls: ['./bot-details.component.css'],
  providers: [AccountService]
})
export class BotDetailsComponent implements OnInit {
  
  BotId: string;
  SpectatorId: string;
  ready: boolean = false;

  constructor(private router: Router, private route: ActivatedRoute, private accountService: AccountService) { }

  ngOnInit() {
    this.route.params.subscribe(data => {
      this.BotId = data['Id']
      console.log(this.BotId);
    });
    this.accountService.getSpectatorId().subscribe(data => {
      this.SpectatorId = data.spectatorId;
      this.ready = true;
    });
  }
}
