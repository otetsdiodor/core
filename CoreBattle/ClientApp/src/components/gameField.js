import React, { Component } from 'react';
import '../custom.css'

export function GameField( props ) {
    const gField = props.field.map(cell => {
        let className = "battle-area-cell";
        if (cell.state == 2 && !props.isEnemy)
            className = "battle-area-cell ship";
        if (cell.state == 3)
            className = "battle-area-cell injured";
        if (cell.state == 1)
            className = "battle-area-cell miss";

        if (props.withClick == false) {
            return <div className={className} pos-x={cell.x} pos-y={cell.y}></div>
        }
        return <div className={className} pos-x={cell.x} pos-y={cell.y} onClick={() => props.onClick(cell.x, cell.y)}></div>
    }
        )

    return ( 
        <div>{gField}</div>
    )
}