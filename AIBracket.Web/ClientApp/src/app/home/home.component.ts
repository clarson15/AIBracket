import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChild('myCanvas') canvasRef: ElementRef;

  ngOnInit() {
    let ctx: CanvasRenderingContext2D =
      this.canvasRef.nativeElement.getContext('2d');
    ctx.beginPath();
    ctx.moveTo(250, 60);
    ctx.lineTo(63.8, 126.4);
    ctx.lineTo(92.2, 372.6);
    ctx.lineTo(250, 460);
    ctx.lineTo(407.8, 372.6);
    ctx.lineTo(436.2, 126.4);
    ctx.moveTo(250, 104.2);
    ctx.lineTo(133.6, 365.2);
    ctx.lineTo(177, 365.2);
    ctx.lineTo(200.4, 306.8);
    ctx.lineTo(299.2, 306.8);
    ctx.lineTo(325.2, 365.2);
    ctx.lineTo(362.6, 365.2);
    ctx.lineTo(250, 104.2);
    ctx.moveTo(304, 270.8);
    ctx.lineTo(216, 270.8);
    ctx.lineTo(250, 189);
    ctx.lineTo(284, 270.8);
    ctx.clip('evenodd');
    ctx.beginPath();
    ctx.fillStyle = '#DD0031';
    for (let i = 0; i < 50000; i++) {
      let x = Math.random() * 500;
      let y = Math.random() * 500;
      ctx.moveTo(x, y);
      ctx.arc(x, y, 1, 0, Math.PI * 2);
    }
    ctx.fill();
  }
}
