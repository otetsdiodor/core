let xp1 = null;
let yp1 = null;
let xp2 = null;
let yp2 = null;
$(document).ready(
    function placeForce() {
        for (let i = 0; i < 10; i++) {
            for (let j = 0; j < 10; j++) {
                let cell = `<div class="battle-area-cell"
                    pos-x="${j}" pos-y="${i}"></div>`;
                $('.player-field').find($('.battle-area-table')).append(cell);
                $('.enemy-field').find($('.battle-area-table')).append(cell);
            }
        }

        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/game")
            .withAutomaticReconnect()
            .build();

        hubConnection.on("Send", function (data) {

            let elem = document.createElement("p");
            elem.appendChild(document.createTextNode(data));
            let firstElem = document.getElementById("chatroom").firstChild;
            document.getElementById("chatroom").insertBefore(elem, firstElem);
        });
        hubConnection.on("PlaceShipResult", function (data, d2) {
            console.log(d2);
            console.log(data);
            let elem = document.createElement("p");
            elem.appendChild(document.createTextNode(data));
            let firstElem = document.getElementById("chatroom").firstChild;
            document.getElementById("chatroom").insertBefore(elem, firstElem);
            FillField('.player-field', d2);
        });

        function FillField(Fname, data) {
            data.forEach(function (d) {
                if (d.state == 2) {
                    $(Fname).find($(`.battle-area-cell[pos-x="${d.y}"][pos-y="${d.x}"]`)).addClass('dead');
                }
            });
        }
        hubConnection.on("ErrorHandler", function (data) {
            let elem = document.createElement("p");
            elem.appendChild(document.createTextNode(data));
            let firstElem = document.getElementById("chatroom").firstChild;
            document.getElementById("chatroom").insertBefore(elem, firstElem);
        });

        hubConnection.on("Your_Turn", function (data) {
            alert('YOUR TURN');
        });

        hubConnection.on("ReadyResult", function (data) {
            console.log(data);
            if (data == 'OK') {
                let elem = document.createElement("p");
                elem.appendChild(document.createTextNode('PLAYER READY'));
                let firstElem = document.getElementById("chatroom").firstChild;
                document.getElementById("chatroom").insertBefore(elem, firstElem);
                $('.player-field').find($('.battle-area-cell')).unbind('click');
                $('#readyBtn').remove();
            }
        });


        hubConnection.on("START_GAME", function () {
            console.log("I AM READY TO FIGHT");
            $('.enemy-field').find($('.battle-area-cell')).bind('click', ShootClick);
        });

        hubConnection.on("EndGame", function (data) {
            $('.enemy-field').find($('.battle-area-cell')).unbind('click');
            if (data == 'WIN') {
                alert('YOU WIN');
            }
            else {
                alert('YOU LOSE!!!!!!!!!!!!!!!');
            }
        });


        hubConnection.on("ShootResult", function (m, en) {
            FillShooted('.player-field', m);
            FillShooted('.enemy-field', en);
        });

        function FillShooted(Fname, data) {
            data.forEach(function (d) {
                let cell = $(Fname).find($(`.battle-area-cell[pos-x="${d.y}"][pos-y="${d.x}"]`));
                if (d.state == 1) {
                    cell.addClass('miss');
                }
                if (d.state == 3) {
                    cell.removeClass('dead');
                    cell.addClass('injured');
                }
            });
        }

        $('#readyBtn').click(function () {
            hubConnection.invoke("Ready", '@ViewBag.gameId');
        });

        $('.player-field').find($('.battle-area-cell')).click(function () {
            let [xPos, yPos] = [this.attributes[1].value, this.attributes[2].value];
            if (xp1 == null && yp1 == null) {
                xp1 = xPos;
                yp1 = yPos;
            }
            else {
                let gameId = '@ViewBag.gameId';
                hubConnection.invoke("PlaceShip", gameId, yp1, xp1, yPos, xPos);
                xp1 = null;
                yp1 = null;
            }
        });

        function ShootClick() {
            let [xPos, yPos] = [this.attributes[1].value, this.attributes[2].value];
            let gameId = '@ViewBag.gameId';
            hubConnection.invoke("Shoot", gameId, yPos, xPos);
        }

        hubConnection.start().catch(function (err) {
            return console.error(err.toString());
        });
    });