import axios from 'axios';
import React, { Component } from 'react';
import { Route } from 'react-router';
import { Link, Switch } from 'react-router-dom';
import ServerHandler from "./ServerHandler.jsx";

export default class Home extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Servers: [],
      StatusServers: [],
      currentInterval: null
    }
  }

  componentDidMount = () => {
    this.GetServerInfo();
    let obj = setInterval(() => {
      this.GetServerInfo();
    }, 5000);
    this.setState({currentInterval: obj});
  }

  GetServerInfo = () =>
  {
    axios.get(window.location.origin + "/api/Server/getserversinfo").then(res => {
      let servers = res.data
      this.setState({ Servers: servers });
    })
  }

  componentWillUnmount = () => {
    clearInterval(this.state.currentInterval);
  }

  renderServers = () => {
    return this.state.Servers.map((item, key) => {
      let obj = (
        <div className="server-manipulate-block" key={key}>
          <h5 style={{ fontWeight: "bold" }}>Имя сервера: <span style={{ textDecoration: "underline", color: "#AA0000", fontWeight: "bold" }}>{item.serverName}</span></h5>
          <br/>
          <h6>{"IP сервера: " + item.serverIp}</h6>
          <h6><span>Статус: {item.isWorking == true ? <span style={{color:"#55FF55"}}>Работает</span> : <span style={{color:"#FF5555"}}>Выключен</span>}</span></h6>
          <h6>{"Онлайн: " + item.currentOnline + " из " + item.maxOnline}</h6>
          <h6>{"Версия сервера: " + item.serverVersion}</h6>
          <h6>{"MOTD сервера: " + item.serverMotd}</h6>
          <br />
          <div className="server-links">
            <Link className="btn btn-outline-secondary" to={"/server/" + item.serverName + "/console"} >Консоль</Link>
            <Link className="btn btn-outline-info" to={"/server/" + item.serverName + "/schematic"} >Схематики</Link>
          </div>
        </div>
      );
      return obj;
    });
  }

  render() {
    return (
      <div>
        <h3>Выбор серверов: </h3>
        <br />
        <div className="server-handlers">
          {this.renderServers()}
        </div>
      </div>
    );
  }
}
