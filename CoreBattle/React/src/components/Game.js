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
        oponentName: "___________",
        playerName: ""
    }

    componentDidMount = () => {
        let conn = new signalR.HubConnectionBuilder()
            .withUrl("/game")
            .build();

        conn.on("Alerter", function (msg) {
            alert(msg);
        });

        conn.on("PlaceShipResult", (fiel) => {
            console.log(fiel);
            this.setState({
                myField: fiel
            });
        });

        conn.on("ReadyResult", (data) => {
            if (data === 'OK') {
                this.setState({
                    isReady: true
                });
            }
        });

        conn.on("MyName", (name) => {
            this.setState({
                playerName: name,
            });
        });
        conn.on("OponentName", (name) => {
            this.setState({
                oponentName: name
            });
        });

        conn.on("ShootResult", (myF, enF) => {
            this.setState({
                myField: myF,
                enemyField: enF
            });
        });

        conn.on("EndGame", (msg) => {
            alert('YOU ' + msg);
            this.setState({
                isStart: false
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

    GetEmptyField = () => {
        let field = [];
        for (var i = 0; i < 10; i++) {
            for (var j = 0; j < 10; j++) {
                let f = {};
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

    PlaceShipClick = (x, y) => {
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
        this.state.hubConnection.invoke("Shoot", x, y);
    }

    render() {
        let field = this.GetEmptyField();
        if (this.state.myField.length !== 0)
            field = this.state.myField;

        let enF = this.GetEmptyField();
        if (this.state.enemyField.length !== 0)
            enF = this.state.enemyField;
        return (
            <div className="Container">
                <div className="row justify-content-center align-items-center player-field battle-area">
                    <div className="col">
                        <h2 className="centerBlock">{this.state.playerName}</h2>
                        <div className="battle-area-table centerBlock">
                            {
                                this.state.isReady && (
                                    <GameField field={field} withClick={false} isEnemy={false} />
                                ) ||
                                !this.state.isReady && (
                                    <GameField field={field} onClick={this.PlaceShipClick} withClick={true} isEnemy={false} />
                                )
                            }
                        </div>
                        {
                            !this.state.isReady && (
                                <input className="btn btn-dark" onClick={this.ReadyClick} type="submit" value="READY TO PLAY" />
                            )
                        }
                    </div>
                </div>

                <div className="row justify-content-center align-items-center enemy-field battle-area">
                    <div className="col">
                        <h2 className="centerBlock">{this.state.oponentName}</h2>
                        <div className="battle-area-table centerBlock">
                            {
                                this.state.isStart && (
                                    <GameField field={enF} withClick={true} onClick={this.ShootClick} isEnemy={true} />
                                ) ||
                                !this.state.isStart && (
                                    <GameField field={enF} withClick={false} isEnemy={true} />
                                )
                            }
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
