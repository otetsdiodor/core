import { Component, OnInit } from '@angular/core';
import * as Signal from '@microsoft/signalr'

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {

  myField;
  HubConnection: Signal.HubConnection;
  StartConnection = () =>{
    this.HubConnection = new Signal.HubConnectionBuilder()
    .withUrl('/game')
    .build();

    this.HubConnection.on("ErrorHandler", function (data){
      console.log("DATA",data);
      alert(data);
    });
    this.HubConnection.on("PlaceShipResult", function (data, d2) {

    });
    this.HubConnection.on("Your_Turn", function (data) {});

    this.HubConnection.on("ReadyResult", function (data) {});

    this.HubConnection.on("START_GAME", function () {});

    this.HubConnection.on("EndGame", function (data) {});

    this.HubConnection.on("ShootResult", function (m, en) {});

    this.HubConnection.start().then(() => console.log('Connection started'))
    .catch(err => console.log('Error while starting connection: ' + err));
    // this.HubConnection.invoke("Send", "MSG");
  }

  constructor() {
    this.StartConnection();
    this.myField = this.GetField();
   }

  ngOnInit() {
  }

  GetField = () => {
    let field = Array<Cell>();
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            let f = new Cell;
            f.x = i;
            f.y = j;
            f.state = 0
            field.push(f);
        }
    }
    return field;
  }
}

class Cell {
  x: number;
  y: number;
  state: number;
}
