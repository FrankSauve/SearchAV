import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Link } from 'react-router-dom';

interface State {
    title: string,
    showDropdown: boolean,
    unauthorized: boolean,
}

export class FileCard extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            title: this.props.title,
            showDropdown: false,
            unauthorized: false
        }
    }

    // Add event listener for a click anywhere in the page
    componentDidMount() {
        document.addEventListener('mouseup', this.hideDropdown);
    }
    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.hideDropdown);
    }

    public deleteWords = () => {

        console.log("Deleting words");

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.delete('/api/word/DeleteByFileId/' + this.props.fileId, config)
            .then(res => {
                console.log(res.status);
                this.deleteVersion();
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public deleteVersion = () => {

        console.log(this.props.fileId);

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }

        axios.delete('/api/version/DeleteFileVersions/' + this.props.fileId, config)
            .then(res => {
                console.log(res.data);
                this.deleteFile();
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public deleteFile = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.delete('/api/file/Delete/' + this.props.fileId, config)
            .then(res => {
                console.log(res.data);
                alert("File deleted");
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    

    public showDropdown = () => {
        this.setState({ showDropdown: true });
    }

    public hideDropdown = () => {
        this.setState({ showDropdown: false });
    }

    public render() {
        return (
            <div className="column is-3">
                <div className="card mg-top-30 fileCard">

                    <span className="tag is-danger is-rounded">{this.props.flag}</span>

                    <header className="card-header">

                        <p className="card-header-title fileTitle">
                            {this.state.title ? this.state.title.substring(0, this.state.title.lastIndexOf('.')) : null}</p>

                        <div className={`dropdown ${this.state.showDropdown ? "is-active" : null}`} >
                            <div className="dropdown-trigger">
                                <div className="is-black" aria-haspopup="true" aria-controls="dropdown-menu4" onClick={this.showDropdown}>
                                    <i className="fas fa-ellipsis-v "></i>
                                </div>
                            </div>

                            <div className="dropdown-menu" id="dropdown-menu4" role="menu">
                                <div className="dropdown-content">
                                    <a className="dropdown-item" onClick={this.deleteWords}>
                                        Effacer le fichier
                                    </a>
                                </div>
                            </div>

                        </div>


                    </header>

                    <div className="card-image">
                        <div className="hovereffect">
                            <figure className="image is-4by3">
                                <img src={this.props.image} alt="Placeholder image" />
                                <div className="overlay">
                                    <Link className="info" to={`/FileView/${this.props.fileId}`}>View/Edit</Link>
                                </div>
                            </figure>
                        </div>
                    </div>

                    <div className="card-content">

                        <div className="content fileContent">
                            <p className="transcription">{this.props.transcription}</p>
                            <p><b>{this.props.username}</b></p>
                            <time dateTime={this.props.date}>{this.props.date}</time>
                            {/* <p><b>Description:</b> {this.props.description}</p> */}
                        </div>
                    </div>

                </div>
            </div>
        );
    }
}
