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
    //console.log(data);
    var values = data.split(' ');
    if (values.length != 21) {
      console.log('something happened');
      console.log(data);
    }
    this.score = Number(values[0]);
    this.lives = Number(values[1]);
    this.pacmanx = Number(values[2]);
    this.pacmany = Number(values[3]);
    this.ghost1x = Number(values[4]);
    this.ghost1y = Number(values[5]);
    this.ghost1d = Boolean(values[6]);
    this.ghost1i = Boolean(values[7]);
    this.ghost2x = Number(values[8]);
    this.ghost2y = Number(values[9]);
    this.ghost2d = Boolean(values[10]);
    this.ghost2i = Boolean(values[11]);
    this.ghost3x = Number(values[12]);
    this.ghost3y = Number(values[13]);
    this.ghost3d = Boolean(values[14]);
    this.ghost3i = Boolean(values[15]);
    this.ghost4x = Number(values[16]);
    this.ghost4y = Number(values[17]);
    this.ghost4d = Boolean(values[18]);
    this.ghost4i = Boolean(values[19]);
  }

  send(message: string) {
    this.socket$.send(message);
  }
}
