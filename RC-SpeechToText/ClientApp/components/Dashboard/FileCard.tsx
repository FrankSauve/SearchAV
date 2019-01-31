import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth'; 

export class FileCard extends React.Component<any> {
    constructor(props: any) {
        super(props);
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
                <div className="card mg-top-30">
                    <header className="card-header">
                        {this.props.flag != null ? <span className="tag is-danger">{this.props.flag}</span> : null}
                        <p className="card-header-title">
                            {this.props.title}
                        </p>
                   
                </header>
                    <div className="card-image">
                        <div className="hovereffect">
                    <figure className="image is-4by3">
                                <img src={this.props.image} alt="Placeholder image" />
                                <div className="overlay">
                                    <a className="info" href="/FileView">View/Edit</a>
                                </div>
                            </figure>
                        </div>
                    </div>
                    <div className="card-content">

                        <div className="content">
                            <p>{this.props.transcription}</p>
                            <p><b>{this.props.username}</b></p>
                            <time dateTime={this.props.date}>{this.props.date}</time>
                        </div>
                    </div>
                    <footer className="card-footer">
                        <a className="card-footer-item" onClick={this.deleteFileWithVersions}>Delete</a>
                    </footer>
                </div>
            </div>
        );
    }
}