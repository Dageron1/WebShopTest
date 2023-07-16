// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function receiveaddnewRoomMessage(maxRoom, roomId, roomName, userId) {
    var newli = document.createElement("li");

    /*let ui = li.parentNode;*/
    let ui = document.getElementById("ulroomTabs");
    let li = document.getElementById("liaddnewRoom");

    var roomHeight = 450;
    var deleteicon = "";
    var changeTab = false;

    if (typeof li != "undefined" && li != null) {
        roomHeight = 280;
        deleteicon = `<i class="bi bi-trash text-danger deleteRoom" onclick="deleteRoom(${roomId},'${roomName}')"></i>`;

        if (li.firstElementChild.classList.contains("active")) {
            changeTab = true;
        }
    } else {
        if (ui.children.length == 0) {
            changeTab = true;
        }
    }

    if (userId == document.getElementById("hdUserId").value) {
        changeTab = true;
    }

    newli.innerHTML = `<a class="nav-link text-center" id="room${roomId}-tab" data-bs-toggle="tab"
                            href="#room${roomId}" role="tab" aria-controls="room${roomId}"
                            aria-selected="true">
                                ${roomName} ${deleteicon}
                                </a>`;

    newli.classList.add("nav-item");
    newli.classList.add("w-20");

    newli.setAttribute("role", "presentation");

    if (changeTab) {
        newli.firstElementChild.classList.add("active");
    }

    ui.appendChild(newli);

    if (typeof li != "undefined" && li != null) {
        ui.removeChild(li);
        ui.appendChild(li);
    }

    let newdiv = document.createElement("div");
    var divrooms = document.getElementById("divRooms");

    newdiv.classList.add("tab-pane");
    newdiv.classList.add("fade");

    newdiv.classList.add("h-100");

    newdiv.setAttribute("id", `room${roomId}`);

    newdiv.setAttribute("role", "tabpanel");

    newdiv.setAttribute("aria-labelledby", `room${roomId}-tab`);

    if (ui.childElementCount > maxRoom) {
        if (typeof li != "undefined" && li != null) {
            li.classList.add("d-none");
        }
    }

    var serchaInput = "";
    if (typeof li != "undefined" && li != null) {
        serchaInput = `<div class="flex-column">
                                            <div class="row g-3 align-items-center" style="position: static;">
                                                <div class="col-auto">
                                                    <label for="inputMessage${roomId}" class="col-form-label">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="26px" height="26px" fill="currentColor" class="bi bi-emoji-smile" viewBox="0 0 16 16">
                                        <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"></path>
                                        <path d="M4.285 9.567a.5.5 0 0 1 .683.183A3.498 3.498 0 0 0 8 11.5a3.498 3.498 0 0 0 3.032-1.75.5.5 0 1 1 .866.5A4.498 4.498 0 0 1 8 12.5a4.498 4.498 0 0 1-3.898-2.25.5.5 0 0 1 .183-.683zM7 6.5C7 7.328 6.552 8 6 8s-1-.672-1-1.5S5.448 5 6 5s1 .672 1 1.5zm4 0c0 .828-.448 1.5-1 1.5s-1-.672-1-1.5S9.448 5 10 5s1 .672 1 1.5z"></path>
                                    </svg>
                                                    </label>
                                                  </div>
                                                  <div class="col" style="position: static;">
                                                    <textarea placeholder="Type your message..." style="resize:none; resize:none;" type="text" id="inputMessage${roomId}" onkeyup="readypublicMessage(${roomId})" class="form-control"></textarea>
                                                  </div>
                                                  <div class="col-auto">
                                                      <button type="button" disabled
                                                        id="btnMessage${roomId}"
                                                      onclick="sendpublicMessage(${roomId})" class="btn btn-primary">Send</button>
                                                  </div>
                                            </div>
                                        </div>`;
    }

    newdiv.innerHTML = `<div class="container  h-100"><div class="row h-100 flex-column p-3"> 
                    <div class="flex-fill border border-dark rounded" style="overflow:hidden; hyphens: auto; overflow-wrap: break-word">
                        <div class="d-block" id="divChatbox${roomId}" style="overflow-y:auto; max-height: ${roomHeight}px overflow-wrap: break-word">
                            <ul class="p-2" style="list-style-type:none; overflow-wrap: break-word" id="ulmessagesList${roomId}">
                            </ul>
                        </div>
                    </div>
                        ${serchaInput}
                                        
            </div></div>`;

    divrooms.appendChild(newdiv);

    setTimeout(function () {
        /*li.firstElementChild.classList.remove('active');*/

        if (changeTab) {
            var childli = ui.getElementsByTagName("li");
            var childDivs = divrooms.getElementsByTagName("div");

            for (i = 0; i < childDivs.length; i++) {
                childDivs[i].classList.remove("show");
                childDivs[i].classList.remove("active");
            }

            newdiv.classList.add("active");
            newdiv.classList.add("show");

            for (i = 0; i < childli.length; i++) {
                childli[i].firstElementChild.classList.remove("active");
            }

            newli.firstElementChild.classList.add("active");
        }
    }, 100);

    /*li.classList.remove('active');*/
}

function receivepublicMessage(roomId, userId, username, message) {
    let ulmessagesList = document.getElementById(`ulmessagesList${roomId}`);

    let li = document.createElement("li");
    let newmsg = document.createElement("p");

    if (
        userId == document.getElementById("hdUserId").value ||
        document.getElementById("hdUserId").value == ""
    ) {
        newmsg.innerHTML = `${username}: ${message}`;
    } else {
        newmsg.innerHTML = `<i role="button" class="bi bi-arrow-right-circle text-primary" onclick="openprivateChat('${userId}','${username}')"> </i> ${username}: ${message}`;
    }

    li.appendChild(newmsg);
    ulmessagesList.appendChild(li);

    li.scrollIntoView(false);
    li.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });
}

function readypublicMessage(roomId) {
    let inputMsg = document.getElementById(`inputMessage${roomId}`);
    let sendButton = document.getElementById(`btnMessage${roomId}`);

    if (/^\s/.test(inputMsg.value)) {
        inputMsg.value = '';
        sendButton.disabled = true;
    }
    else {
        sendButton.disabled = false;
    }
}

function receiveprivateMessage(
    senderId,
    senderName,
    receiveId,
    message,
    chatId,
    receiverName,
    time,
    isReaded
) {
    var chatboxName = `ulmessagesList${receiveId}`;

    if (receiveId === document.getElementById("hdUserId").value) {
        chatboxName = `ulmessagesList${senderId}`;
    }

    let ulmessagesList = document.getElementById(chatboxName);

    if (typeof ulmessagesList == "undefined" || ulmessagesList == null) {
        receiveopenprivateChat(senderId, senderName);
        ulmessagesList = document.getElementById(chatboxName);
    }
    let myAudio = document.querySelector('#audio');

    let li = document.createElement("li");
    let newmsg = document.createElement("p");

    newmsg.setAttribute("id", chatId);

    newmsg.classList.add("alert");
    newmsg.classList.add("px-2");
    ulmessagesList.classList.add("p-2");


    if (senderId === document.getElementById("hdUserId").value) {
        li.classList.add("text-end");
        newmsg.classList.add("me-3");
        newmsg.classList.add("p-2");
        newmsg.classList.add("inline");
        newmsg.classList.add("text-break");
        newmsg.classList.add("text-start");
        newmsg.classList.add("inline");
        newmsg.classList.add("alert-primary");
        newmsg.classList.add("border1");
        newmsg.classList.add("wrap");
        newmsg.innerHTML = `${message}<button role="button" align="left" style="background:none; border:none;" class="bi bi-trash-fill text-danger" onclick="deleteprivateChat('${chatId}')"></button><div id="checkMark${senderId}" class="test${senderId} text-end bi bi-check"> ${time}</div>`;
        
    } else {
        li.classList.add("text-start");
        newmsg.classList.add("ms-1");
        newmsg.classList.add("p-2");
        newmsg.classList.add("alert-info");
        newmsg.classList.add("inline2");
        newmsg.classList.add("text-break");
        newmsg.classList.add("wrap");
        newmsg.classList.add("border2");
        myAudio.play();
        newmsg.innerHTML = `${message}<div class="text-start test"> ${time} <i role="button" class="bi bi-trash text-danger" onclick="deleteprivateChat('${chatId}')"></i></div>`;
        notifyPMMessage(1, senderId);   
    }

    li.appendChild(newmsg);


    ulmessagesList.appendChild(li);

    li.scrollIntoView(false);
    li.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });

    notifyUnreadMessage(1, isReaded, senderId, receiveId);
    
}

function readyprivateMessage() {
    let inputMsg = document.getElementById("inputMessagePrivate");

    let sendButton = document.getElementById("btnMessagePrivate");
    if (/^\s/.test(inputMsg.value)) {
        inputMsg.value = '';
        sendButton.disabled = true;
    } else {
        sendButton.disabled = false;
    }
}

function setchatuserId(userId) {
    document.getElementById("hdchatUserId").value = userId;

    notifyPMMessage(0, userId);
}

function receivedeleteRoomMessage(deleted, selected) {
    let ui = document.getElementById("ulroomTabs");
    let link = document.getElementById(`room${deleted}-tab`);

    let li = link.parentNode;
    /*let ui = li.parentNode;*/

    let divrooms = document.getElementById("divRooms");
    let childdiv = document.getElementById(`room${deleted}`);

    var changetab = true;
    if (link.classList.contains("active") == false) {
        changetab = false;
    }

    ui.removeChild(li);
    divrooms.removeChild(childdiv);

    setTimeout(function () {
        var childli = ui.getElementsByTagName("li");
        var childDivs = divrooms.getElementsByTagName("div");

        let liaddnew = document.getElementById("liaddnewRoom");

        if (changetab) {
            for (i = 0; i < childDivs.length; i++) {
                childDivs[i].classList.remove("show");
                childDivs[i].classList.remove("active");
            }

            for (i = 0; i < childli.length; i++) {
                childli[i].firstElementChild.classList.remove("active");
            }
        }

        if (Number(selected) > 0) {
            if (changetab) {
                let newdiv = document.getElementById(`room${selected}`);
                let link1 = document.getElementById(`room${selected}-tab`);

                let newli = link1.parentNode;

                newdiv.classList.add("active");
                newdiv.classList.add("show");

                newli.firstElementChild.classList.add("active");
            }

            liaddnew.classList.remove("d-none");
        } else {
            //newli.classList.add('show');
            //newli.classList.add('active');

            liaddnew.firstElementChild.classList.add("active");
        }
    }, 100);
}

function receiveopenprivateChat(userId, userName, tabchange) {
    let pubtab = document.getElementById("public-tab");
    let pribtab = document.getElementById("private-tab");

    let pubtabbox = document.getElementById("publictabbox");
    let pribtabbox = document.getElementById("privatetabbox");

    if (tabchange) {
        pubtab.classList.remove("active");
        pribtab.classList.add("active");

        pubtabbox.classList.remove("show");
        pubtabbox.classList.remove("active");

        pribtabbox.classList.add("show");
        pribtabbox.classList.add("active");
    }

    document.getElementById("hdchatUserId").value = userId;

    let div = document.getElementById("list-tab");

    var newa = document.getElementById(`list-${userId}-list`);
    if (typeof newa != "undefined" && newa != null) {
        // Exists.
        newa.classList.add("active");
    } else {
        var children = div.children;
        for (var i = 0; i < children.length; i++) {
            children[i].classList.remove("active");
        }

        newa = document.createElement("a");

        newa.classList.add("list-group-item");
        newa.classList.add("list-group-item-action");
        newa.classList.add("active");

        newa.setAttribute("id", `list-${userId}-list`);

        newa.setAttribute("data-bs-toggle", `list`);
        newa.setAttribute("href", `#list-${userId}`);
        newa.setAttribute("role", `tab`);
        newa.setAttribute("aria-controls", `list-${userId}`);

        newa.setAttribute("onclick", `setchatuserId('${userId}')`);

        newa.innerHTML = `<div class="row">
                        <div class="col" onclick="readMessage('${userId}')"> 
                            <i role="button" class="bi bi-x-lg" title="Close chat" onclick="deleteprivatechatGroup('${userId}')"> </i> ${userName}
                        </div>
                        <div class="col-auto">
                            <i class="dot block-text align-middle bg-success"
                                id="spanOnline${userId}" style="margin-left:5px;" title="Online">                               
                                </i>
                                <span id="user-badge${userId}" class="badge bg-danger"></span>
                        </div>
                    </div>`;

        div.appendChild(newa);
    }

    let div1 = document.getElementById("nav-tabContent");

    var div2 = document.getElementById(`list-${userId}`);

    if (typeof div2 != "undefined" && div2 != null) {
        // Exists.
        div2.classList.add("active");
        div2.classList.add("show");
    } else {
        var children = div1.children;
        for (var i = 0; i < children.length; i++) {
            children[i].classList.remove("active");
            children[i].classList.remove("show");
        }

        div2 = document.createElement("div");

        div2.classList.add("tab-pane");
        div2.classList.add("fade");
        div2.classList.add("active");
        div2.classList.add("show");

        div2.setAttribute("id", `list-${userId}`);

        div2.setAttribute("aria-labelledby", `list-${userId}-list`);
        div2.setAttribute("role", `tabpanel`);

        div2.innerHTML = `<div style="overflow: hidden;"> 
                        <div class="d-block px-2 pb-2" /* style="overflow-y:auto*/; max-height:1700px">  
                            <ul class="p-2" style="list-style-type:none; /*overflow-wrap: break-word*/" id="ulmessagesList${userId}">
                            </ul>
                        </div>
                    </div>`;

        div1.appendChild(div2);
    }
}

function receivedeleteprivateChat(chatid) {
    let p = document.getElementById(chatid);
    let myAudioDelete = document.querySelector('#deleteSound');
    let li = p.parentNode;
    let ui = li.parentNode;

    ui.removeChild(li);
    myAudioDelete.play();

    notifyUnreadMessage(2);

}


function setchatboxColor(elementid, color) {
    document.getElementById("private-badge").innerText = "";
    document.getElementById(elementid).style.backgroundColor = color;
}

function deleteprivatechatGroup(userId) {
    var selectedDelete = false;
    let div = document.getElementById("list-tab");

    var newa = document.getElementById(`list-${userId}-list`);
    if (typeof newa != "undefined" && newa != null) {
        if (newa.classList.contains("active")) {
            selectedDelete = true;
        }

        div.removeChild(newa);
    }

    let div1 = document.getElementById("nav-tabContent");

    var div2 = document.getElementById(`list-${userId}`);

    if (typeof div2 != "undefined" && div2 != null) {
        div1.removeChild(div2);
    }

    if (div.children.length == 0) {
        let pubtab = document.getElementById("public-tab");
        let pribtab = document.getElementById("private-tab");

        pubtab.classList.add("active");
        pribtab.classList.remove("active");

        let pubtabbox = document.getElementById("publictabbox");
        let pribtabbox = document.getElementById("privatetabbox");

        pubtabbox.classList.add("show");
        pubtabbox.classList.add("active");

        pribtabbox.classList.remove("show");
        pribtabbox.classList.remove("active");
    } else {
        if (selectedDelete) {
            div.children[0].classList.remove("show");
            div.children[0].classList.remove("active");
            div1.children[0].classList.remove("active");

            document.getElementById("hdchatUserId").value = div1.children[0]
                .getAttribute("id")
                .replace("list-", "");
        }
    }
}

function notifyUnreadMessage(action, isReaded, senderId, receiverId) {
    let pubtab = document.getElementById("public-tab");
    let badge = document.getElementById("private-badge");
    let privuser = document.getElementById(`list-${senderId}-list`);
    let divid = document.getElementById(`list${senderId}`);


    if (pubtab.classList.contains("active")) {
        var badgeval = Number(badge.innerText);
        if (action == 1) {
            badgeval = badgeval + 1;
        } else {
            badgeval = badgeval - 1;
        }

        if (badgeval == 0) {
            badge.innerText = "";
        } else if (badgeval <= 99) {
            badge.innerText = badgeval;
        } else {
            badge.innerText = "99+";
        }
        
    }
    if (privuser.classList.contains("active")) {
        var elements = document.getElementsByClassName(`test${receiverId}`);
        for (var i = 0; i < elements.length; i++) {
            elements[i].classList.remove("bi-check");
            elements[i].classList.add("bi-check-all");
        }

    }

    //if (divid.classList.contains("active")) {
    //    var elements = document.getElementsByClassName(`test${senderId}`);
    //    for (var i = 0; i < elements.length; i++) {
    //        elements[i].classList.remove("bi-check");
    //        elements[i].classList.add("bi-check-all");
    //    }
    //}


    if (!privuser.classList.contains("active")) {
        var elements = document.getElementsByClassName(`test${receiverId}`);
        for (var i = 0; i < elements.length; i++) {
            elements[i].classList.remove("bi-check");
            
        }

    }
}
function notifyPMMessage(action, userId) {
    let badge = document.getElementById(`user-badge${userId}`);
    let listUser = document.getElementById(`list-${userId}-list`);

    if (!listUser.classList.contains("active")) {
        var badgeval = Number(badge.innerText);
        if (action == 1) {
            badgeval = badgeval + 1;
        } else {
            badgeval = badgeval - 1;
        }

        if (badgeval == 0) {
            badge.innerText = "";
        } else if (badgeval <= 99) {
            badge.innerText = badgeval;
        } else {
            badge.innerText = "99+";
        }
       
    }
    if (listUser.classList.contains("active")) {
        var badgeval = Number(badge.innerText);

        badgeval = 0;

        if (badgeval == 0) {
            badge.innerText = "";
            
        } else if (badgeval <= 99) {
            badge.innerText = badgeval;
        } else {
            badge.innerText = "99+";
        }
        
        
       
    }
}



