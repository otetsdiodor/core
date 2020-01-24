import React, { Component } from 'react';
import './custom.css'
import * as signalR from "@microsoft/signalr";

//export class gameField(props) extends Component {

//    render() {
//        return (
//            <div class="player-field battle-area">
//                <div class="battle-area-table">
//                </div>
//                <input class="btn btn-dark" id="readyBtn" type="submit" value="READY TO PLAY" />
//            </div>
//        );
//    }
//}

export default function gameField({ field }) {
    const gField = field.map(cell =>
        <div key={field.id} className="battle-area-cell" pos-x={cell.X} pos-y={cell.Y}></div>
    )

    return ( 
            {gField}
    )
}