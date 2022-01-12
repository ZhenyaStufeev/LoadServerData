import React, { Component } from 'react';

export default class Icons extends Component {
  RenderIcons = () => {
    return this.props.icons.map((val, key) => {
      let obj = (
        <div style={{ 
          padding: "15px", 
          border: "0.5px solid silver", 
          borderRadius: "5px", 
          display: "flex", 
          flexDirection: "column",
          alignItems: "center",
          minWidth: "120px" }}>
          <div className={val} style={{ fontSize: "50px" }} />
          <div>{val}</div>
        </div>
      )
      return obj;
    });
  }

  render() {
    console.log(this.props);
    return (
      <div className="iconShow" style={{display: "flex", flexWrap: "wrap"}}>
        {this.RenderIcons()}
      </div>
    );
  }
}
