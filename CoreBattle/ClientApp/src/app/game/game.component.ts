import { Component, OnInit } from '@angular/core';
import * as Signal from '@microsoft/signalr'

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {

  y1;
  x1;
  isReady:boolean;
  isStart:boolean;
  myName:string;
  opName:string;
  myField: Cell[];
  enField: Cell[];
  HubConnection: Signal.HubConnection;
  StartConnection = () =>{
    this.HubConnection = new Signal.HubConnectionBuilder()
    .withUrl('/game')
    .build();

    this.HubConnection.on("ErrorHandler", (data) => {
      alert(data);
    });
    this.HubConnection.on("PlaceShipResult", (data) => {
      this.myField = data;
    });
    this.HubConnection.on("MyName", (data) => {
      this.myName = data;
    });

    this.HubConnection.on("OponentName", (data) => {
      this.opName = data;
    });
    
    this.HubConnection.on("ReadyResult", (data) => {
      if(data == "OK")
          this.isReady = true; 
    });

    this.HubConnection.on("START_GAME", () => {
      this.isStart = true;
    });

    this.HubConnection.on("EndGame", (data) => {
      alert(data);
      this.isStart = false;
    });

    this.HubConnection.on("ShootResult",  (m, en) => {
      console.log("MY",this.myField)
      this.myField = m;
      console.log("ENEMY",this.enField)
      this.enField = en;
    });

    this.HubConnection.start().then(() => console.log('Connection started'))
    .catch(err => console.log('Error while starting connection: ' + err));
    // this.HubConnection.invoke("Send", "MSG");
  }

  constructor() {
    this.isReady = false;
    this.isStart = false;
    this.StartConnection();
    this.myField = this.GetField();
    this.enField = this.GetField();
   }

  ngOnInit() {
  }

  PlaceShipClick = (x,y) => {
    if (this.x1 == null && this.y1 == null) {
        this.x1= x;
        this.y1= y;       
    }
    else {
        this.HubConnection.invoke("PlaceShip", this.x1, this.y1, x, y);
        this.x1 = null;
        this.y1 = null;
    }
  }

  ShootClick = (x, y) => {
    this.HubConnection.invoke("Shoot",x,y);
  }

  ReadyClick = () => {
    this.HubConnection.invoke("Ready");
  }

  GetField = () => {
    let field = Array<Cell>();
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            let f = new Cell;
            f.id = this.uuidv4();
            f.x = i;
            f.y = j;
            f.state = 0
            field.push(f);
        }
    }
    return field;
  }
  CellClass(state:number,isEnemy:boolean){
    if(state == 1){
      return "battle-area-cell miss";
    }
    if(state == 2 && !isEnemy){
      return "battle-area-cell ship";
    }
    if(state == 3){
      return "battle-area-cell injured";
    }
    return "battle-area-cell";
  }
  public trackItem (index: number, item: Cell) {
    return item.id;
  }
  uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}

class Cell {
  id: string;
  x: number;
  y: number;
  state: number;
}