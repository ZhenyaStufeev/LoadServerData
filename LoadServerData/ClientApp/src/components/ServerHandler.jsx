import axios from 'axios';
import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Redirect } from "react-router-dom"


const ServerHandler = (props) => {
  const [isConnected, setIsConnected] = useState(true);
  const [isNotCorrect, setIsNotCorrect] = useState(false);
  const [serverName, setServerName] = useState("");
  const [inputText, setInputText] = useState("");
  const [autoScroll, setAutoScroll] = useState(true);
  const [serverStatus, setServerStatus] = useState({
    currentOnline: -1,
    isWorking: false,
    maxOnline: -1,
    serverIp: "Не известно",
    serverVersion: "Не известно",
    serverMotd: "Не известно",
    serverName: "",
    allocatedMemmory: 0,
    memmoryUsage: 0
  });
  const [lastCountOfMessages, setLastCountOfMessages] = useState(0);
  const mountedRef = useRef(false); // Добавляем useRef для отслеживания размонтирования компонента

  const connectionRef = useRef(null);

  const Receive = (servername) => {
    connectionRef.current.on(servername, data => {
      let count_of_messages = lastCountOfMessages;
      count_of_messages += 1;
      let parent_out = document.getElementById("ServerConsoleOut");
      parent_out.innerHTML += "<li>" + data + "</li>";
      setLastCountOfMessages(count_of_messages);
      if (autoScroll)
        parent_out.scrollTop = parent_out.scrollHeight;
    })
  }

  const ConnectSignalRServer = () => {
    connectionRef.current
      .start()
      .then(() => {
        console.log("CONNECTED");
      })
      .catch((err) => {
        setIsConnected(false);
      });
  };

  const DisconnectSignalRServer = () => {
    if (connectionRef.current && connectionRef.current.state === "Connected") {
      connectionRef.current
        .stop()
        .then(() => {
          console.log("DISCONNECTED");
        })
        .catch((err) => {
          console.log(err);
        });
    }
  };

  useEffect(() => {
    mountedRef.current = true; // Устанавливаем значение true при монтировании компонента
    let server = window.location.pathname;
    server = server.replace("/server/", "").replace("/console", "");
    let interv = null;
    if (server.length === 0) {
      setIsNotCorrect(true);
    } else {
      connectionRef.current = new HubConnectionBuilder()
        .withUrl("/signalr")
        .build();

      connectionRef.current.onclose(() => {
        setIsConnected(false);
      });

      ConnectSignalRServer();

      let decodedUrl = decodeURI(server);

      if (connectionRef.current) {
        connectionRef.current.off();
      }

      Receive(decodedUrl);
      GetStatus(server);
      interv = setInterval(() => {
        GetStatus(server);
      }, 5000);

      axios.post(window.location.origin + "/api/Server/subscribesignalrserver", { ServerName: decodedUrl }).then(() => {
      });
      setServerName(decodedUrl);
    }

    return () => {
      mountedRef.current = false; // Устанавливаем значение false при размонтировании компонента
      clearInterval(interv);
      DisconnectSignalRServer();

      if (connectionRef.current) {
        connectionRef.current.off();
        connectionRef.current.stop();
      }

      console.log("DISCONNECT");
    };
  }, []);

  useEffect(() => {
    if (autoScroll === true) {
      let parent_out = document.getElementById("ServerConsoleOut");
      parent_out.scrollTop = parent_out.scrollHeight;
    }
  }, [lastCountOfMessages]);

  const onStartServer = () => {
    axios.post(window.location.origin + "/api/Server/runserver", { ServerName: serverName })
      .then(p => {
        if (p.data === true)
          props.sendNotify("tr", "success", "Сервер успешно запущен", "pe-7s-power");
        else
          props.sendNotify("tr", "error", "Произошла ошибка", "pe-7s-tools");
      })
      .then(() => {
        axios.post(window.location.origin + "/api/Server/subscribesignalrserver", { ServerName: serverName })
          .then(() => {
            if (mountedRef.current) { // Проверяем, существует ли компонент перед обновлением состояния
              setServerStatus(prevStatus => ({
                ...prevStatus,
                isWorking: true
              }));
            }
          });
      });
  };

  const onStopServer = () => {
    axios.post(window.location.origin + "/api/Server/stopserver", { ServerName: serverName })
      .then(p => {
        if (p.data === true)
          props.sendNotify("tr", "success", "Сервер успешно остановлен", "pe-7s-power");
        else
          props.sendNotify("tr", "error", "Произошла ошибка", "pe-7s-tools");
      })
      .then(() => {
        axios.post(window.location.origin + "/api/Server/subscribesignalrserver", { ServerName: serverName })
          .then(() => {
            if (mountedRef.current) { // Проверяем, существует ли компонент перед обновлением состояния
              setServerStatus(prevStatus => ({
                ...prevStatus,
                isWorking: false
              }));
            }
          });
      });
  };

  const sendCommandStop = () => {
    props.sendNotify("tr", "success", "Команда на выполение остановки сервера отправлена. Ожидайте ответа сервера.", "pe-7s-power");
    SendMessage("stop");
  }

  const areaOnChange = (e) => {
    setInputText(e.target.value);
  }

  const OnKeyDown = (e) => {
    if (e.keyCode === 13 && e.shiftKey === false) {
      e.preventDefault();
      SendMessage(inputText);
    }
  }

  const SendMessage = (message) => {
    setInputText("");
    return axios.post(window.location.origin + "/api/Server/sendcommand", { ServerName: serverName, Command: message });
  }

  const GetStatus = (serverName) => {
    axios.get(window.location.origin + "/api/Server/getserverstatus/" + serverName)
      .then(p => {
        if (mountedRef.current) { // Проверяем, существует ли компонент перед обновлением состояния
          setServerStatus(p.data);
        }
      });
  };

  if (isNotCorrect)
    return <Redirect to="/" />;

  return (
    <div>
      {isNotCorrect === true ? <Redirect to="/" /> : ""}
      <h5 style={{ fontWeight: "bold" }}>Консоль сервера: <span style={{ textDecoration: "underline", color: "#AA0000", fontWeight: "bold" }}>{serverStatus.serverName}</span></h5>
      <br />
      <h6>{"IP сервера: " + serverStatus.serverIp}</h6>
      <h6><span>Статус: {serverStatus.isWorking === true ? <span style={{ color: "#55FF55" }}>Работает</span> : <span style={{ color: "#FF5555" }}>Выключен</span>}</span></h6>
      <h6>{"Онлайн: " + serverStatus.currentOnline + " из " + serverStatus.maxOnline}</h6>
      <h6>{"Версия сервера: " + serverStatus.serverVersion}</h6>
      <h6>{"MOTD сервера: " + serverStatus.serverMotd}</h6>
      <h6>{"Выделено ОЗУ: " + (serverStatus.allocatedMemmory / (1024 * 1024)) + " МБ"}</h6>
      <h6>{"Потребление ОЗУ: " + (serverStatus.memmoryUsage / (1024 * 1024)) + " МБ"}</h6>
      <br />

      <ul className="ServerConsoleOut" id="ServerConsoleOut">
        {/* Консоль */}
      </ul>

      <div className="console-input-block">
        <div className="input-control">
          <textarea className="form-control" placeholder="Ввести команду (Enter для отправки сообщения)" value={inputText} onKeyDown={OnKeyDown} onChange={areaOnChange}></textarea>
          <div onClick={() => SendMessage(inputText)} className="pe-7s-paper-plane sendButton"></div>
        </div>
        <div className="control">
          <div className="buttons">
            <button className="btn btn-success" onClick={onStartServer} disabled={serverStatus.isWorking}>Запустить сервер</button>
            <button className="btn btn-warning" onClick={sendCommandStop} disabled={!serverStatus.isWorking}>Безопасное отключение сервера</button>
            <button className="btn btn-danger" onClick={onStopServer} disabled={!serverStatus.isWorking}>Экстренное выключение сервера</button>
          </div>
          <div className="form-check">
            <input onChange={() => {
              setAutoScroll(!autoScroll);
            }} checked={autoScroll} className="form-check-input" type="checkbox" value="" id="flexCheckDefault" />
            <label className="form-check-label" htmlFor="flexCheckDefault">
              Авто-просмотр новых сообщений
            </label>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ServerHandler;
