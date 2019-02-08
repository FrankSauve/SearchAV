import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { TranscriptionText } from './TranscriptionText';
import { VideoPlayer } from './VideoPlayer';
import { DescriptionText } from './DescriptionText';
import { TranscriptionSearch } from './TranscriptionSearch';
import { Link } from 'react-router-dom';

interface State {
    fileId: number,
    version: any,
    file: any,
    unauthorized: boolean,
    fileTitle: string,
    description: any
}

export default class FileView extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            fileId: this.props.match.params.id,
            version: null,
            file: null,
            unauthorized: false,
            fileTitle: "",
            description: null
        }
    }

    // Called when the component is rendered
    public componentDidMount() {
        this.getVersion();
        this.getFile();
    }

    public getVersion = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/version/GetByFileId/' + this.state.fileId, config)
            .then(res => {
                this.setState({ version: res.data[0] });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getFile = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/details/' + this.state.fileId, config)
            .then(res => {
                this.setState({ file: res.data });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });                
                }
            });
    }

    public handleChange = (event: React.FormEvent<HTMLTextAreaElement>) => {
        var safeSearchTypeValue: string = event.currentTarget.value;

        this.setState({ description: safeSearchTypeValue });
    }


    render() {
        return (
            <div className="container">
                <div className="columns">
                    <div className="column is-one-third mg-top-30">
                        {/* Using title for now, this will have to be change to path eventually */}
                        {this.state.file ? <VideoPlayer path={this.state.file.title} /> : null}

                        <p><b>Titre: </b>{this.state.file ? (this.state.file.title ? <div className="card">
                            <div className="card-content">
                                {this.state.file.title}
                            </div> </div> : <div className="card">
                                <div className="card-content"> This file has no title </div></div>) : null}</p>

                        <br/>

                        <p><b>Description: </b>{this.state.file ? (this.state.file.description ? <div className="card">
                            <div className="card-content">
                            {this.state.file.description}
                            </div> </div> : <div className="card">
                                <div className="card-content"> This file has no description </div></div> ) : null}</p>
                    </div>

                    <div className="column mg-top-30">
                        {this.state.version ? <TranscriptionSearch versionId={this.state.version.id}/> : null }
                        {this.state.version ? <TranscriptionText text={this.state.version.transcription} /> : null}
                    </div>
                    <div className="column mg-top-30">
                    </div>
                </div>
            </div>

        )
    }
}
