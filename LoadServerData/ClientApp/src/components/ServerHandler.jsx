import axios from 'axios';
import React, { Component, useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Redirect } from "react-router-dom"
import { Router } from "react-router";
let connection = new HubConnectionBuilder()
  .withUrl("/signalr")
  .build();

export default class ServerHandler extends Component {

  constructor(props) {
    super(props);
    this.state = {
      messages: [],
      IsConnected: true,
      IsNotCorrect: false,
      ServerName: "",
      inputText: "",
      autoScroll: true,
      ServerStatus: {
        currentOnline: -1,
        isWorking: false,
        maxOnline: -1,
        serverIp: "Не известно",
        serverVersion: "Не известно",
        serverMotd: "Не известно",
        serverName: this.ServerName
      },
      lastCountOfMessages: 0,
      currentInterval: null
    }
    //    this.Receive("Mohist 1.12.2");

    this.Receive = this.Receive.bind(this);
    this.ConnectSignalRServer = this.ConnectSignalRServer.bind(this);
    this.DisconnectSignalRServer = this.DisconnectSignalRServer.bind(this);
    // this.OpenOrCloseConnection(true);
    //  this.ConnectSignalRServer();
  }

  Receive = (servername) => {
    connection.on(servername, data => {
      let message_array = this.state.messages;
      message_array.push(data);

      this.setState({ messages: message_array });
    })
  }

  ConnectSignalRServer() {
    connection.start().then(() => {
      this.setState({ IsConnected: true });
      console.log("CONNECTED");
    }).catch((err) => {
      this.setState({ IsConnected: false })
    });

  }

  DisconnectSignalRServer() {
    if (this.state.IsConnected === true) {
      connection.stop().then().catch((err) => console.log(err));
      this.setState({ IsConnected: false });
    }
  }

  componentDidMount = () => {
    let server = window.location.pathname;
    server = server.replace("/server/", "").replace("/console", "");
    if (server.length == 0) {
      this.setState({ IsNotCorrect: true })
    }
    else {
      this.ConnectSignalRServer();
      let decodedUrl = decodeURI(server);
      this.Receive(decodedUrl);
      this.GetStatus(server);
      let interv = setInterval(() => {
        this.GetStatus(server);
      }, 5000);
      this.setState({ currentInterval: interv });

      axios.post(window.location.origin + "/api/Server/subscribesignalrserver", { ServerName: decodedUrl }).then(() => {
      })
      this.setState({ ServerName: decodedUrl });
    }
  }

  GetStatus = (server) => {
    axios.get(window.location.origin + "/api/Server/getserverstatus/" + server).then(status_result => {
      let st_obj = {
        currentOnline: status_result.data.currentOnline,
        isWorking: status_result.data.isWorking,
        maxOnline: status_result.data.maxOnline,
        serverIp: status_result.data.serverIp,
        serverVersion: status_result.data.serverVersion,
        serverMotd: status_result.data.serverMotd,
        serverName: status_result.data.serverName
      };
      this.setState({ ServerStatus: st_obj });
    });
  }

  RenderMessages() {
    let messages = this.state.messages;
    let res_messages = [];
    for (let i = 0; i < messages.length; ++i) {
      let li;
      if ((messages.length - 1) === i) {
        li = <li key={i} dangerouslySetInnerHTML={{ __html: messages[i] }} id="last-message"></li>
      }
      else {
        li = <li key={i} dangerouslySetInnerHTML={{ __html: messages[i] }}></li>
      }
      res_messages.push(li);
    }
    return res_messages;
  }

  OnKeyDown = (e) => {
    if (e.keyCode === 13 && e.shiftKey === false) {
      e.preventDefault();
      this.SendMessage(this.state.inputText );
    }
  }

  SendMessage = (message) =>
  {
    this.setState({ inputText: "" })
    return axios.post(window.location.origin + "/api/Server/sendcommand", { ServerName: this.state.ServerName, Command: message});
  }

  areaOnChange = (e) => {
    this.setState({ inputText: e.target.value });
  }

  onStartServer = () => {
    axios.post(window.location.origin + "/api/Server/runserver", { ServerName: this.state.ServerName }).then(p => {
      console.log(p);
      if (p.data === true)
        this.props.sendNotify("tr", "success", "Сервер успешно запущен", "pe-7s-power");
      else
        this.props.sendNotify("tr", "error", "Произошла ошибка", "pe-7s-tools");

    }).then(() => {
      axios.post(window.location.origin + "/api/Server/subscribesignalrserver", { ServerName: this.state.ServerName }).then(() => {
      })
    });
  }


  OnStopServer = () => {
    axios.post(window.location.origin + "/api/Server/killserver", { ServerName: this.state.ServerName }).then(p => {
      if (p.data === true) {
        this.props.sendNotify("tr", "warning", "Процесс сервера был убит. (Экстренное выключение)", "pe-7s-power");
      }
      else {//info, warning, error, success
        this.props.sendNotify("tr", "error", "Произошла ошибка", "pe-7s-tools");
      }
    });
  }

  componentDidUpdate = () => {
    if (this.state.autoScroll === true) {
      let last_message = document.getElementById("last-message");
      if (last_message != null) {
        if (this.state.lastCountOfMessages != this.state.messages.length) {
          last_message.scrollIntoView({ behavior: "smooth" });
          this.setState({ lastCountOfMessages: this.state.messages.length })
        }
      }
    }
  }

  componentWillUnmount = () => {
    clearInterval(this.state.currentInterval);
  }

SendCommandStop = () =>
{
  this.props.sendNotify("tr", "success", "Команда на выполение остановки сервера отправлена. Ожидайте ответа сервера.", "pe-7s-power");
  this.SendMessage("stop");
}

  render() {

    let item = this.state.ServerStatus;
    return (
      <div>
        {this.state.IsNotCorrect === true ? <Redirect to="/" /> : ""}
        <h5 style={{ fontWeight: "bold" }}>Консоль сервера: <span style={{ textDecoration: "underline", color: "#AA0000", fontWeight: "bold" }}>{item.serverName}</span></h5>
        <br />
        <h6>{"IP сервера: " + item.serverIp}</h6>
        <h6><span>Статус: {item.isWorking == true ? <span style={{ color: "#55FF55" }}>Работает</span> : <span style={{ color: "#FF5555" }}>Выключен</span>}</span></h6>
        <h6>{"Онлайн: " + item.currentOnline + " из " + item.maxOnline}</h6>
        <h6>{"Версия сервера: " + item.serverVersion}</h6>
        <h6>{"MOTD сервера: " + item.serverMotd}</h6>
        <br />

        <ul className="ServerConsoleOut">
          {this.RenderMessages()}
        </ul>
        <div className="console-input-block">
          <div className="input-control">
            <textarea className="form-control" placeholder="Ввести команду (Enter для отправки сообщения)" value={this.state.inputText} onKeyDown={this.OnKeyDown} onChange={this.areaOnChange}></textarea>
            <div onClick={() => this.SendMessage(this.state.inputText)} className="pe-7s-paper-plane sendButton"></div>
          </div>
          <div className="control">
            <div className="buttons">
              <button className="btn btn-success" onClick={this.onStartServer} disabled={this.state.ServerStatus.isWorking}>Запустить сервер</button>
              <button className="btn btn-warning" onClick={this.SendCommandStop} disabled={this.state.ServerStatus.isWorking == false ? true : false}>Безопасное отключение сервера</button>
              <button className="btn btn-danger" onClick={this.OnStopServer} disabled={this.state.ServerStatus.isWorking == false ? true : false}>Экстренное выключение сервера</button>
            </div>
            <div class="form-check">
              <input onChange={() => {
                this.setState({ autoScroll: !this.state.autoScroll })
              }} checked={this.state.autoScroll} class="form-check-input" type="checkbox" value="" id="flexCheckDefault" />
              <label class="form-check-label" for="flexCheckDefault">
                Авто-просмотр новых сообщений
              </label>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
