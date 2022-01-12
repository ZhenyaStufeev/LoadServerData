
import axios from "axios";
import React, { Component } from "react";
import { withRouter } from "react-router";
import { Redirect } from "react-router-dom"
// import Button from "reactstrap/lib/Button";
// import 
// import FormGroup from "reactstrap/lib/FormGroup";
import ListGroup from "reactstrap/lib/ListGroup";
import ListGroupItem from "reactstrap/lib/ListGroupItem";
import loading from './loading.gif'
class ShowInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            items: [],
            currentlyFiles: [],
            uploads: [],
            isUploading: false,
            ServerName: "",
            IsNotCorrect: false,
            inputValuUploader: ""
        };
    }



    DeleteFile = async(server_name, file_name) =>
    {
        axios.delete(window.location.origin + "/file/" + server_name + "/" + file_name).then(() => {
            this.GetFiles(server_name);
        });
    }

    renderItems = () => {
        return this.state.items.map((item, key) => {
            return <ListGroupItem key={key}>{item.name}<button onClick={() => this.DeleteFile(this.state.ServerName, item.name)} className="btn btn-outline-danger">Удалить</button></ListGroupItem>
        });
    }

    GetFiles = async (servername) => {
        axios.get(window.location.origin + "/file/getschematics/"+servername).then(res => {
            console.log(res.data);
            let temp = res.data.map(item => {
                return { name: item };
            });
            this.setState({ items: temp });
        });
    }

    componentDidMount = () => {
        let server = window.location.pathname;
        server = server.replace("/server/", "").replace("/schematic", "");
        if (server.length == 0) {
            this.setState({ IsNotCorrect: true })
        }
        else
        {
            let decodedUrl = decodeURI(server);
            this.setState({ ServerName: decodedUrl });
        this.GetFiles(decodedUrl);
        }
    }

    uploadFile = async () => {
        let files = this.state.currentlyFiles;
        let uploads = this.state.uploads.map(item => {
            item.isLoading = true
            return item;
        })

        this.setState({ isLoading: true, uploads: uploads });
        for (let i = 0; i < files.length; ++i) {
            let formData = new FormData();
            formData.append("FileName", files[i].name)
            formData.append("FormFile", files[i]);
            formData.append("ServerName", this.state.ServerName);

            await axios.post(window.location.origin + "/file", formData).then(res => {

            });
        }
        this.setState({ isLoading: false, uploads: [], currentlyFiles: [], inputValuUploader: "" });
        this.GetFiles(this.state.ServerName);
    }

    onChange = (e) => {
        let files = e.target.files;
        let uploads = [];
        for (let i = 0; i < files.length; ++i) {
            uploads.push({ file_name: files[i].name, isLoading: false });
        }
        
        this.setState({ currentlyFiles: files, uploads: uploads, inputValuUploader: e.target.value });
    }

    render() {
        return (
            <div>
                {this.state.IsNotCorrect === true ? <Redirect to="/" /> : ""}
                <h3>Работа из shematics <span style={{textDecoration: "underline", color: "#AA0000", fontWeight: "bold"}}>{this.state.ServerName}</span></h3>
                <br/>
                <p></p>
                <div className="update-block">
                    <button type="button" className="btn btn-outline-info" onClick={() => this.GetFiles(this.state.ServerName)}>Обновить данные</button>
                </div>
                <p></p>
                <ListGroup variant="flush" className="schem-list">
                    {this.renderItems()}
                </ListGroup>
                <p></p>
                <div class="custom-file">
                    <p style={{ alignSelf: "baseline" }}><b>Можно грузить много!</b></p>
                    <input type="file" id="schematicInput" value={this.state.inputValuUploader} onChange={this.onChange} accept=".schematic" className="form-control" id="customFile" multiple />
                    <div style={{width:"100%"}}>
                        <p></p>
                        <ListGroup variant="flush" className="schem-list">
                            {
                                this.state.uploads.map((item, key) => {

                                    return <ListGroupItem className="list-item" key={key}>
                                        {item.file_name}
                                        {item.isLoading === true ? <img src={loading} style={{ height: "25px" }} /> : <></>}
                                    </ListGroupItem>
                                })
                            }
                        </ListGroup>
                    </div>
                    <button type="button" disabled={this.setState.isUploading === true ? true : false} className="btn btn-outline-success" onClick={this.uploadFile}>Загрузить</button>
                </div>
                <br/>
                <br/>
            </div>
        );
    }
}

export default withRouter(ShowInfo);
