import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

export interface PacmanEvent {
  name: string;
  enum: number;
  example: string;
  description: string;
}

const EVENT_DATA: PacmanEvent[] = [
  {
    name: "Board Reset", enum: 0, example: "0 28 31 0 0 1 1 5...", description: "Packet is recieved when the game is started or the board is reset. The first two numbers are width and height of the board " 
      + " followed by a 2D matrices of tile enumerations read from left to right, top to bottom. 0 is a wall, 1 is a blank, 2 is a dot, 3 is a fruit, 4 is a power up, and 5 is a teleportation tile."},
  { name: "Pacman Position", enum: 1, example: "1 12 14", description: "Packet is recieved whenever Pacman moves and is followed by the new X and Y position." },
  {
    name: "Ghost Update", enum: 2, example: "2 2 10 10 False False", description: "Packet is recieved every game tick and is followed by a 0-3 specifying which ghost, the ghosts X and Y position after moving, "
  + "and two booleans indicating whether the ghost is dead(true)/alive(false) and is vulnerable(true)/invulnerable(false) to dying respectively." },
  { name: "Dot Consumed", enum: 3, example: "3 4 22 10", description: "Packet is recieved whenever a dot is consumed and is followed by the X and Y position of said dot and a score increment." },
  { name: "Fruit Consumed", enum: 4, example: "4 15 4", description: "Packet is recieved whenever a fruit is consumed and is followed by the X and Y position of said fruit and a score increment." },
  { name: "Power Up Consumed", enum: 5, example: "5 2 18", description: "Packet is recieved whenever a power up is consumed and is followed by the X and Y position of said power up and a score increment." },
  { name: "Fruit Spawn", enum: 6, example: "6 22 22", description: "Packet is recieved whenever a new fruit spawns and is followed by the X and Y position where it spawned." },
  { name: "Pacman Lives", enum: 7, example: "7 2", description: "Packet is recieved whenever Pacman dies and is followed by the new lives value." },
  { name: "Ghost Spawn/Despawn", enum: 8, example: "8 1 True", description: "Packet is recieved whenever a ghost spawns in or dies and is followed by a 0-3 specifying which ghost, boolean indicating whether it is despawning(true)/spawning(false)." },
  { name: "Score", enum: 9, example: "1 12 14", description: "Packet is recieved at the start of the game and is followed by the score. This is intended for specators." }
];

@Component({
  selector: 'getting-started',
  templateUrl: './getting-started.component.html',
  styleUrls: ['./getting-started.component.css']
})
export class GettingStartedComponent implements OnInit {

  constructor(private router: Router) { }
  displayedColumn: string[] = ['name', 'enum', 'example'];
  dataSource = EVENT_DATA;
  ngOnInit() {
  }
}
