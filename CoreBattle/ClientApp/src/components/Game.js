import React, { Component } from 'react';
import { GameField } from './gameField';
import '../custom.css'
import * as signalR from "@microsoft/signalr";

export class Game extends Component {
    state = {
        myField: [],
        enemyField: [],
        hubConnection: null,
        x1: null,
        y1: null,
        isReady: false,
        isStart: false,
        isEnded: false,
        oponent: "___________",
        player: ""
    }

    componentDidMount = () => {
        let conn = new signalR.HubConnectionBuilder()
            .withUrl("/game")
            .build();

        conn.on("GetGameIdResult", function (gameid) {
            this.setState({
                gameId: gameid
            })
        });

        conn.on("ErrorHandler", function (msg) {
            alert(msg);
        });

        conn.on("PlaceShipResult", (fiel) => {
            this.setState({
                myField: fiel
            });
        });

        conn.on("ReadyResult", (data) => {
            if (data == 'OK') {
                this.setState({
                    isReady: true
                });
            }
        });

        conn.on("MyName", (name) => {
            this.setState({
                player: name,
            });
        });
        conn.on("OponentName", (name) => {
            this.setState({
                oponent: name
            });
        });
        
        conn.on("ShootResult", (myF,enF) => {
            this.setState({
                myField: myF,
                enemyField: enF
            });
        });

        conn.on("EndGame", (msg) => {
            alert('YOU ' + msg);
            this.setState({
                isEnded: true
            });
        });

        conn.on("START_GAME", () => {
            this.setState({
                isStart: true
            });
        });

        conn.start();
        this.setState({
            hubConnection: conn
        })
    }

    GetField = () => {
        let field = Array();
        for (var i = 0; i < 10; i++) {
            for (var j = 0; j < 10; j++) {
                let f = new Object();
                f.x = i;
                f.y = j;
                f.state = 0
                field.push(f);
            }
        }
        return field;
    }

    ReadyClick = () => {
        this.state.hubConnection.invoke("Ready");
    }

    PlaceShipClick = (x,y) => {
        if (this.state.x1 == null && this.state.y1 == null) {
            this.setState({
                x1: x,
                y1: y
            })
        }
        else {
            this.state.hubConnection.invoke("PlaceShip", this.state.x1, this.state.y1, x, y);
            this.setState({
                x1: null,
                y1: null
            })
        }
    }

    ShootClick = (x, y) => {
        this.state.hubConnection.invoke("Shoot",x,y);
    }

    render() {
        let field = null;
        if (this.state.myField.length == 0) {
            field = this.GetField();
        }
        else {
            field = this.state.myField;
        }
        let inputReady = null;
        let gameBoard = <GameField field={field} withClick={false} isEnemy={false}/>
        if (this.state.isReady != true) {
            gameBoard = <GameField field={field} onClick={this.PlaceShipClick} withClick={true} isEnemy={false} />
            inputReady = <input className="btn btn-dark" onClick={this.ReadyClick} id="readyBtn" type="submit" value="READY TO PLAY" />
        }
        let enF = this.GetField();
        if (this.state.enemyField.length != 0) {
            enF = this.state.enemyField;
        }
        let enemyF = <GameField field={enF} withClick={false} isEnemy={true}/>
        if (this.state.isStart) {
            enemyF = <GameField field={enF} withClick={true} onClick={this.ShootClick} isEnemy={true}/>
        }
        if (this.state.isEnded) {
            enemyF = <GameField field={enF} withClick={false} isEnemy={true}/>;
            gameBoard = <GameField field={field} isEnemy={false} withClick={false} />;
        }
        return (
            <div className="Container">
                <div className="row justify-content-center align-items-center player-field battle-area">
                    <div className="col">
                        <h2 className="centerBlock">{this.state.player}</h2>
                        <div className="battle-area-table centerBlock">
                            {gameBoard}
                        </div>
                        {inputReady}
                    </div>
                </div>

                <div className="row justify-content-center align-items-center enemy-field battle-area">
                    <div className="col">
                        <h2 className="centerBlock">{this.state.oponent}</h2>
                        <div className="battle-area-table centerBlock">
                            {enemyF}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
