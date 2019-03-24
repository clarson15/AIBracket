import { Component, ViewChild, ElementRef, OnInit, NgZone } from '@angular/core';
import { GameService } from '../services/game.service';

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

  constructor(private gameservice: GameService, private ngZone: NgZone) { }

ngOnInit() {
    var ctx = this.canvasRef.nativeElement.getContext('2d');
    this.socket$ = new WebSocket("ws://localhost:8000");
    this.socket$.onmessage = (event) => {
      this.UpdateGame(event.data);
    };
    this.socket$.onopen = (event) => {
      this.gameservice.getFeaturedGame().subscribe(
        data => {
          if (data == '0') {
            this.ActiveGame = false;
          }
          else {
            this.ActiveGame = true;
            this.ActiveGameId = data;
            var msg = 'WATCH ' + this.ActiveGameId;
            this.send(msg);
            this.ngZone.runOutsideAngular(() => this.paintLoop(ctx));
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
    // Paint current frame
    ctx.fillStyle = 'white';
    ctx.clearRect(0, 0, 550, 600);

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
            case 2: //wall blank dot fruit powerup portal
              ctx.fillStyle = 'grey';
              ctx.beginPath();
              ctx.arc(y + 8, x + 8, 2, 0, 6.28);
              ctx.fill();
              break;
            case 3:
              ctx.fillStyle = 'red';
              ctx.beginPath();
              ctx.arc(y + 8, x + 8, 2, 0, 6.28);
              ctx.fill();
              break;
            case 4:
              ctx.fillStyle = 'green';
              ctx.beginPath();
              ctx.arc(y + 8, x + 8, 2, 0, 6.28);
              ctx.fill();
              break;
            case 5:
              ctx.fillStyle = 'purple';
              ctx.beginPath();
              ctx.arc(y + 8, x + 8, 2, 0, 6.28);
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

    ctx.fillStyle = 'yellow';
    ctx.fillRect(this.pacmanx * 16, this.pacmany * 16 + 80, 16, 16);
    ctx.stroke();

    ctx.fillStyle = 'blue';
    ctx.fillRect(this.ghost1x * 16, this.ghost1y * 16 + 80, 16, 16);
    ctx.fillRect(this.ghost2x * 16, this.ghost2y * 16 + 80, 16, 16);
    ctx.fillRect(this.ghost3x * 16, this.ghost3y * 16 + 80, 16, 16);
    ctx.fillRect(this.ghost4x * 16, this.ghost4y * 16 + 80, 16, 16);

    ctx.fillStyle = 'black';
    ctx.font = '30pt Verdana';
    ctx.fillText('Score: ' + this.score, 10, 30);
    ctx.fillText('Lives: ' + this.lives, 10, 60);
    ctx.stroke();

    // Schedule next frame
    requestAnimationFrame(() => this.paintLoop(ctx));
  }

  UpdateGame(data: string) {
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
          let rows = payload.split('\n');
          console.log(rows.length + 'x' + rows[0].split(' ').length);
          rows.forEach(y => {
            let newrow = Array<number>();
            let columns = y.split(' ');
            columns.forEach(x => {
              newrow.push(Number(x));
            });
            this.map.push(newrow);
          });
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
        default:
          break;
      }
    });
  }

  send(message: string) {
    this.socket$.send(message);
  }
}
