import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Link } from 'react-router-dom';

interface State {
    title: string,
    showDropdown: boolean,
    unauthorized: boolean
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

    public deleteFileWithVersions = () => {

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

                axios.delete('/api/file/Delete/' + this.props.fileId, config)
                    .then(res => {
                        console.log(res.data);
                    })
                    .catch(err => {
                        if (err.response.status == 401) {
                            this.setState({ 'unauthorized': true });
                        }
                    });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public render() {
        return (
            <div className="column is-3">
                <div className="card mg-top-30 fileCard">

                    <span className="tag is-danger is-rounded">{this.props.flag}</span>

                    <header className="card-header">

                        <p className="card-header-title fileTitle">
                            {this.state.title.substring(0, this.state.title.lastIndexOf('.'))}</p>

                        <div className="dropdown is-hoverable">
                            <div className="dropdown-trigger">
                                <a  aria-haspopup="true" aria-controls="dropdown-menu4">
                                    <i className="fas fa-ellipsis-v"></i></a>
                            </div>

                           <div className="dropdown-menu" id="dropdown-menu3" role="menu">
                                <div className="dropdown-content">
                                    <a className="dropdown-item" onClick={this.deleteFileWithVersions}>
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
                        </div>
                    </div>

                </div>
            </div>
        );
    }
}
