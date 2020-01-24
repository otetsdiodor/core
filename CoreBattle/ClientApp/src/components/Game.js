import React, { Component } from 'react';
import './custom.css'
import * as signalR from "@microsoft/signalr";

export class Game extends Component {
    stat = {
        gameId:null,
        hubConnection: null
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

        //conn.on("Send", function (msg) {
        //    alert(msg);
        //});


        conn.start();
        this.setState({
            hubConnection: conn
        })
    }

    GetField = () => {
        this.state.hubConnection.invoke("Send", ");
    }

    klick = () => {
        this.state.hubConnection.invoke("Send","MSG");
        //this.state.hubConnection.invoke("GetGameId");
    }


    render() {
        let f = GetField();
        return (
            <div class="player-field battle-area">
                <div class="battle-area-table">
                    <gameField field={f}/>
                </div>
                <input class="btn btn-dark" id="readyBtn" type="submit" value="READY TO PLAY" />
            </div>
        );
    }
}