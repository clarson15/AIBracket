import { Component, OnInit, ViewChild, ElementRef, NgZone, Input, OnDestroy } from '@angular/core';
import { PacmanGhost } from '../models/PacmanGhost';
import { multicast } from 'rxjs/operators';


@Component({
  selector: 'game-pacman',
  templateUrl: './game-pacman.component.html',
  styleUrls: ['./game-pacman.component.css']
})
export class GamePacmanComponent implements OnInit, OnDestroy {

  @ViewChild('myCanvas') canvasRef: ElementRef;

  private socket$: WebSocket;
  @Input()
  SpectatorId: string;
  @Input()
  ActiveGameId: string;

  private activeGame: boolean = true;
  private SocketConnected: boolean;
  private chatbox: HTMLElement;
  private map: Array<Array<number>> = new Array<Array<number>>();
  private score: number;
  private lives: number;
  private pacmanX: number = 0;
  private pacmanY: number = 0;
  private pacmanTargetX: number = 0;
  private pacmanTargetY: number = 0;
  private ghosts: PacmanGhost[];
  private lastFrame: Date;
  private ghostImages: HTMLImageElement[];
  private ghostV: HTMLImageElement;

  constructor(private ngZone: NgZone) {}

  ngOnInit() {
    this.ghostImages = new Array<HTMLImageElement>(4);
    for (let i = 0; i < 4; i++) {
      this.ghostImages[i] = new Image();
      this.ghostImages[i].src = 'assets/Ghost' + (i+1) + '.png';
    }
    this.ghostV = new Image();
    this.ghostV.src = "assets/GhostV.png";
    this.ghosts = new Array<PacmanGhost>();
    for (let i = 0; i < 4; i++) {
      this.ghosts.push(new PacmanGhost());
    }
    var canvas = document.getElementsByTagName('canvas')[0];
    this.chatbox = document.getElementById('chat');
    if (document.body.clientWidth < 550) {
      let ratio = document.body.clientWidth / 550;
      canvas.style.width = document.body.clientWidth + 'px';
      canvas.style.height = 600 * ratio + 'px';
    }
    canvas.width = 448;
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
    this.socket$.onerror = (event) => {
      if (this.SocketConnected == null) {
        this.ngZone.runOutsideAngular(() => this.paintLoop(ctx));
      }
      this.SocketConnected = false;
    };
    this.socket$.onopen = (event) => {
      this.SocketConnected = true;
      var msg = 'WATCH ' + this.ActiveGameId;
      console.log(this.SpectatorId);
      if (this.SpectatorId.length > 0) {
        msg += ' ' + this.SpectatorId;
      }
      this.send(msg);
    };
    this.ngZone.runOutsideAngular(() => this.paintLoop(ctx));
  }

  paintLoop(ctx: CanvasRenderingContext2D) {

    if (ctx == null) {
      return;
    }
    if (this.lastFrame == null) {
      this.lastFrame = new Date();
    }
    let frametime = new Date().getTime() - this.lastFrame.getTime();
    this.lastFrame = new Date();

    ctx.fillStyle = 'white';
    ctx.clearRect(0, 0, 550, 600);
    if (!this.SocketConnected) {
      ctx.fillStyle = 'black';
      ctx.textAlign = 'center';
      ctx.font = '20pt Verdana';
      ctx.fillText('Error connecting to game server.', 275, 300);
      ctx.stroke();
    }
    else if (this.activeGame) {
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
                ctx.arc(y + 8, x + 8, 3, 0, Math.PI * 2);
                ctx.fill();
                break;
              case 3:
                ctx.fillStyle = 'red';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 3, 0, Math.PI * 2);
                ctx.fill();
                break;
              case 4:
                ctx.fillStyle = 'green';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 5, 0, Math.PI * 2);
                ctx.fill();
                break;
              case 5:
                ctx.fillStyle = 'purple';
                ctx.beginPath();
                ctx.arc(y + 8, x + 8, 3, 0, Math.PI * 2);
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
      if (this.pacmanX - this.pacmanTargetX > 0) {
        this.pacmanX -= frametime / 100;
      }
      if (this.pacmanX - this.pacmanTargetX < 0) {
        this.pacmanX += frametime / 100;
      }
      if (this.pacmanY - this.pacmanTargetY > 0) {
        this.pacmanY -= frametime / 100;
      }
      if (this.pacmanY - this.pacmanTargetY < 0) {
        this.pacmanY += frametime / 100;
      }
      ctx.beginPath();
      ctx.arc((this.pacmanX * 16) + 8, (this.pacmanY * 16) + 88, 7, 0.75 * Math.PI, 1.8 * Math.PI, false);
      ctx.fillStyle = "rgb(0, 0, 0)";
      ctx.fill();
      ctx.beginPath();
      ctx.arc((this.pacmanX * 16) + 8, (this.pacmanY * 16) + 88, 7, 0.2 * Math.PI, 1.25 * Math.PI, false);
      ctx.fillStyle = "rgb(0, 0, 0)";
      ctx.fill();
      ctx.beginPath();
      ctx.arc((this.pacmanX * 16) + 8, (this.pacmanY * 16) + 88, 6, 0.25 * Math.PI, 1.25 * Math.PI, false);
      ctx.fillStyle = "rgb(255, 255, 0)";
      ctx.fill();
      ctx.beginPath();
      ctx.arc((this.pacmanX * 16) + 8, (this.pacmanY * 16) + 88, 6, 0.75 * Math.PI, 1.75 * Math.PI, false);
      ctx.fillStyle = "rgb(255, 255, 0)";
      ctx.fill();

      let i = 0;
      this.ghosts.forEach(ghost => {
        ctx.fillStyle = 'blue';
        if (ghost.x - ghost.targetX > 0) {
          ghost.x -= frametime / 100;
        }
        if (ghost.x - ghost.targetX < 0) {
          ghost.x += frametime / 100;
        }
        if (ghost.y - ghost.targetY > 0) {
          ghost.y -= frametime / 100;
        }
        if (ghost.y - ghost.targetY < 0) {
          ghost.y += frametime / 100;
        }
        if (this.ghostImages[i].complete && ghost.vulnerable === false) {
          ctx.drawImage(this.ghostImages[i], ghost.x * 16 + 2, ghost.y * 16 + 82);
        }
        else if (this.ghostV.complete && ghost.vulnerable === true) {
          ctx.drawImage(this.ghostV, ghost.x * 16 + 2, ghost.y * 16 + 82);
        }
        i++;
      });

      ctx.beginPath();
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
      ctx.fillText('Error spectating game', 275, 300);
      ctx.stroke();
    }

    // Schedule next frame
    requestAnimationFrame(() => this.paintLoop(ctx));
  }

  SubmitChat() {
    var chatinput = document.getElementById('chatinput') as HTMLTextAreaElement;
    var data = chatinput.value.trim();
    if (data.length > 0) {
      this.send('CHAT ' + data);
    }
    chatinput.value = '';
  }

  UpdateGame(data: string) {
    if (data === "Game does not exist") {
      this.activeGame = false;
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
          let newPX = Number(vals[0]);
          let newPY = Number(vals[1]);
          if (Math.abs(newPX - this.pacmanTargetX) > 1) {
            this.pacmanX = newPX;
            this.pacmanY = newPY;
          }
          else if (Math.abs(newPY - this.pacmanTargetY) > 1) {
            this.pacmanX = newPX;
            this.pacmanY = newPY;
          }
          else { 
            this.pacmanX = this.pacmanTargetX;
            this.pacmanY = this.pacmanTargetY;
          }
          this.pacmanTargetX = newPX;
          this.pacmanTargetY = newPY;
          break;
        case "2":
          var vals = payload.split(' ');
          var index = Number(vals[0]);
          if (index > 3) break;
          let newX = Number(vals[1]);
          let newY = Number(vals[2]);
          if (Math.abs(newX - this.ghosts[index].targetX) > 1) {
            this.ghosts[index].x = newX;
            this.ghosts[index].y = newY;
          }
          else if (Math.abs(newY - this.ghosts[index].targetY) > 1){
            this.ghosts[index].x = newX;
            this.ghosts[index].y = newY;
          }
          else {
            this.ghosts[index].x = this.ghosts[index].targetX;
            this.ghosts[index].y = this.ghosts[index].targetY;
          }
          this.ghosts[index].targetX = newX;
          this.ghosts[index].targetY = newY;
          var d = vals[3] === 'True' ? true : false;
          var v = vals[4] === 'True' ? true : false;
          this.ghosts[index].dead = d;
          this.ghosts[index].vulnerable = v;
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
    this.socket$.send(message);
  }

  ngOnDestroy() {
    if (this.SocketConnected) {
      this.socket$.close();
    }
  }

}




