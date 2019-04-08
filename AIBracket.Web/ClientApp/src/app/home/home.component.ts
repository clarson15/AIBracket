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
  @ViewChild('myCanvas') canvasRef: ElementRef;

  private socket$: WebSocket;
  private ActiveGame: boolean;
  private ActiveGameId: string;
  private SocketConnected: boolean;
  private chatbox: HTMLElement;
  private leaderboard: any;

  private map: Array<Array<number>> = new Array<Array<number>>();
  private score: number;
  private lives: number;
  private pacmanx: number;
  private pacmany: number;
  private ghost1x: number;
  private ghost1y: number;
  private ghost1i: boolean;
  private ghost1d: boolean;
  private ghost2x: number;
  private ghost2y: number;
  private ghost2i: boolean;
  private ghost2d: boolean;
  private ghost3x: number;
  private ghost3y: number;
  private ghost3i: boolean;
  private ghost3d: boolean;
  private ghost4x: number;
  private ghost4y: number;
  private ghost4i: boolean;
  private ghost4d: boolean;

  constructor(private gameservice: GameService, private ngZone: NgZone, private accountservice: AccountService) { }

  ngOnInit() {
    var canvas = document.getElementsByTagName('canvas')[0];
    this.chatbox = document.getElementById('chat');
    canvas.width = 550;
    canvas.height = 600;
    var ctx = this.canvasRef.nativeElement.getContext('2d');
    let port = 0;
    let protocol = "";
    if (document.location.protocol === 'https:') {
      port = 8005;
      protocol = "wss://";
    }
    else {
      protocol = "ws://";
      port = 8000;
    }
    this.socket$ = new WebSocket(protocol + document.location.hostname + ":" + port);
    this.socket$.onmessage = (event) => {
      this.UpdateGame(event.data);
    };
    this.gameservice.getPacmanLeaderboard().subscribe(
      data => {
        this.leaderboard = data;
        console.log(this.leaderboard);
      },
      err => {
        console.log(err.error);
      });
    this.socket$.onerror = (event) => {
      if (this.SocketConnected == null) {
          this.ngZone.runOutsideAngular(() => this.paintLoop(ctx));
        }
        this.SocketConnected = false;
      };
    this.socket$.onopen = (event) => {
      this.SocketConnected = true;
      this.gameservice.getFeaturedGame().subscribe(
      data => {
        if (data == '0') {
          this.ActiveGame = false;
          this.ngZone.runOutsideAngular(() => this.paintLoop(ctx));
        }
        else {
          this.accountservice.getSpectatorId().subscribe(
            data2 => {
              console.log(data2);
              this.ActiveGame = true;
              this.ActiveGameId = data;
              var msg = 'WATCH ' + this.ActiveGameId;
              if (data2.spectatorId.length > 0) {
                msg += ' ' + data2.spectatorId;
              }
              this.send(msg);
              this.ngZone.runOutsideAngular(() => this.paintLoop(ctx));
            },
            err => {
            });
        }
      },
      err => {
        console.log(err.error);
      });
      };
  }

  paintLoop(ctx: CanvasRenderingContext2D) {
    
    if (ctx == null) {
      return;
    }
    ctx.fillStyle = 'white';
    ctx.clearRect(0, 0, 550, 600);
    if (!this.SocketConnected) {
      ctx.fillStyle = 'black';
      ctx.textAlign = 'center';
      ctx.font = '20pt Verdana';
      ctx.fillText('Error connecting to game server.\n' + this.socket$.url, 275, 300);
      ctx.stroke();
    }
    else if (this.ActiveGame) {
      let x = 80, y = 0;
      if (this.map != null) {
        this.map.forEach(yd => {
          yd.forEach(xd => {
            switch (xd) {
              case 0:
                ctx.fillStyle = 'black';
                ctx.fillRect(y, x, 16, 16);
                break;
              case 1:
                break;
              case 2:
                ctx.fillStyle = 'grey';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 4, 0, Math.PI * 2);
                ctx.fill();
                break;
              case 3:
                ctx.fillStyle = 'red';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 4, 0, Math.PI * 2);
                ctx.fill();
                break;
              case 4:
                ctx.fillStyle = 'green';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 4, 0, Math.PI * 2);
                ctx.fill();
                break;
              case 5:
                ctx.fillStyle = 'purple';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 4, 0, Math.PI * 2);
                ctx.fill();
                break;
              default:
                console.log('???:' + xd);
                break;
            }
            y += 16;
          });
          x += 16;
          y = 0;
        });
      }
      
      ctx.beginPath();
      ctx.arc((this.pacmanx * 16) + 8, (this.pacmany * 16) + 88, 6, 0.25 * Math.PI, 1.25 * Math.PI, false);
      ctx.fillStyle = "rgb(255, 255, 0)";
      ctx.fill();
      ctx.beginPath();
      ctx.arc((this.pacmanx * 16) + 8, (this.pacmany * 16) + 88, 6, 0.75 * Math.PI, 1.75 * Math.PI, false);
      ctx.fill();

      ctx.fillStyle = 'blue';
      ctx.fillRect(this.ghost1x * 16, this.ghost1y * 16 + 80, 16, 16);
      ctx.fillRect(this.ghost2x * 16, this.ghost2y * 16 + 80, 16, 16);
      ctx.fillRect(this.ghost3x * 16, this.ghost3y * 16 + 80, 16, 16);
      ctx.fillRect(this.ghost4x * 16, this.ghost4y * 16 + 80, 16, 16);

      ctx.fillStyle = 'black';
      ctx.font = '30pt Verdana';
      ctx.textAlign = 'start';
      if (this.score != null) {
        ctx.fillText('Score: ' + this.score, 10, 30);
      }
      if (this.lives != null) {
        ctx.fillText('Lives: ' + this.lives, 10, 60);
      }
      ctx.stroke();
    }
    else {
      ctx.fillStyle = 'black';
      ctx.textAlign = 'center';
      ctx.font = '30pt Verdana';
      ctx.fillText('There is no active game', 275, 300);
      ctx.stroke();
    }

    // Schedule next frame
    requestAnimationFrame(() => this.paintLoop(ctx));
  }

  SubmitChat() {
    var chatinput = document.getElementById('chatinput') as HTMLTextAreaElement;
    var data = chatinput.value.trim();
    console.log(data);
    if (data.length > 0) {
      this.send('CHAT ' + data);
    }
    chatinput.value = '';
  }

  UpdateGame(data: string) {
    if (data === "Game does not exist") {
      this.ActiveGame = false;
      return;
    }
    var packets = data.split('*');
    if (packets == null) {
      return;
    }
    packets.forEach(x => {
      var seperator = x.indexOf(' ');
      if (seperator == -1) return;
      var type = x.substr(0, seperator);
      var payload = x.substr(seperator + 1);
      switch (type) {
        case "0":
          this.map.length = 0;
          let tiles = payload.split(' ');
          let width = Number(tiles.shift());
          let height = Number(tiles.shift());
          for (let i = 0; i < height; i += 1) {
            let rows = new Array<number>();
            for (let j = 0; j < width; j += 1) {
              rows.push(Number(tiles[i * width + j]));
            }
            this.map.push(rows);
          }
          break;
        case "1":
          var vals = payload.split(' ');
          this.pacmanx = Number(vals[0]);
          this.pacmany = Number(vals[1]);
          break;
        case "2":
          var vals = payload.split(' ');
          switch (vals[0]) {
            case "0":
              this.ghost1x = Number(vals[1]);
              this.ghost1y = Number(vals[2]);
              this.ghost1d = Boolean(vals[3]);
              this.ghost1i = Boolean(vals[4]);
              break;
            case "1":
              this.ghost2x = Number(vals[1]);
              this.ghost2y = Number(vals[2]);
              this.ghost2d = Boolean(vals[3]);
              this.ghost2i = Boolean(vals[4]);
              break;
            case "2":
              this.ghost3x = Number(vals[1]);
              this.ghost3y = Number(vals[2]);
              this.ghost3d = Boolean(vals[3]);
              this.ghost3i = Boolean(vals[4]);
              break;
            case "3":
              this.ghost4x = Number(vals[1]);
              this.ghost4y = Number(vals[2]);
              this.ghost4d = Boolean(vals[3]);
              this.ghost4i = Boolean(vals[4]);
              break;
            default:
              break;
          }
          break;
        case "3":
        case "4":
        case "5":
          var vals = payload.split(' ');
          this.map[Number(vals[1])][Number(vals[0])] = 1;
          this.score += Number(vals[2]);
          break;
        case "6":
          var vals = payload.split(' ');
          this.map[Number(vals[1])][Number(vals[0])] = 3;
          break;
        case "7":
          var vals = payload.split(' ');
          this.lives = Number(vals[0]);
          break;
        case "8":
          var vals = payload.split(' ');
          // Ghost die
          break;
        case "9":
          var vals = payload.split(' ');
          this.score = Number(vals[0]);
          break;
        case "CHAT":
          var vals = payload.split(':');
          this.chatbox.innerText = this.chatbox.innerText + "\n" + vals[0] + ': ' + vals[1];
        default:
          break;
      }
    });
  }

  send(message: string) {
    console.log('sending ' + message);
    this.socket$.send(message);
  }


}
