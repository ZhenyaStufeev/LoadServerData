import React, { Component } from 'react';
import { Route, Link } from 'react-router';
import { Layout } from './components/Layout';
import Home from "./components/Home";
import ShowInfo from "./components/Show";
import ServerHandler from './components/ServerHandler';
import './custom.css'
import './assets/css/pe-icon-7-stroke.css'
import NotificationSystem from "react-notification-system";
import { style } from "./assets/variables/Variables";
import { iconsArray } from "./assets/variables/Variables";
import Icons from "./components/Icons";

export default class App extends Component {
  static displayName = App.name;

  componentDidMount = () => {
    this.setState({ _notificationSystem: this.refs.notificationSystem });
  }

  handleNotificationClick = (position, level, text, icon) => {
    this.state._notificationSystem.addNotification({
      title: <span data-notify="icon" className={icon} />,
      message: text,
      level: level, //info, warning, error, success
      position: position, //tr,tl
      autoDismiss: 10 //время
    });
  };


  render() {
    return (
      <Layout>
        <NotificationSystem ref="notificationSystem" style={style} />
        <Route exact path='/' render={(props) => {return <Home {...props} sendNotify={this.handleNotificationClick}/>}} />
        <Route path='/server/*/console' render={(props) => {return <ServerHandler {...props} sendNotify={this.handleNotificationClick}/>}}></Route>
        <Route path='/server/*/schematic' render={(props) => {return <ShowInfo {...props} sendNotify={this.handleNotificationClick}/>}}></Route>
        <Route path='/secter/icons' render={(props) => { return <Icons {...props} icons={iconsArray }/>}}></Route>
      </Layout>
    );
  }
}
