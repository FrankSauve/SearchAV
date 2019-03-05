import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { TranscriptionText } from './TranscriptionText';
import { VideoPlayer } from './VideoPlayer';
import { DescriptionText } from './DescriptionText';
import { TranscriptionSearch } from './TranscriptionSearch';
import { SaveTranscriptionButton } from './SaveTranscriptionButton';
import { Link } from 'react-router-dom';
import Loading from '../Loading';

interface State {
    fileId: number,
    version: any,
    file: any,
    user: any,
    editedTranscript: string,
    unauthorized: boolean,
    fileTitle: string,
    description: any,
    loading: boolean
}

export default class FileView extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            fileId: this.props.match.params.id,
            version: null,
            file: null,
            user: null,
            editedTranscript: "",
            unauthorized: false,
            fileTitle: "",
            description: null,
            loading: false
        }
    }

    // Called when the component is rendered
    public componentDidMount() {
        this.getVersion();
        this.getFile();
        this.getUser();
    }

    public getVersion = () => {
        this.setState({ loading: true });
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/version/GetActiveByFileId/' + this.state.fileId, config)
            .then(res => {
                this.setState({ version: res.data });
                this.setState({ editedTranscript: this.state.version.transcription }); //Will avoid to have empty transcript except if user erase everything
                this.setState({ loading: false });
            })
            .catch(err => {
                console.log(err);
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
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });                
                }
            });
    }

    public getUser = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/user/getUserByEmail/' + auth.getEmail(), config)
            .then(res => {
                this.setState({ user: res.data });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public handleTranscriptChange = (event: any) => {
        this.setState({ editedTranscript: event.target.value });
    }

    public updateVersion = (newVersion: any) => {
        this.setState({ version: newVersion });
    };

    render() {
        return (
            <div className="container">

                <div className="columns">
                    <div className="column is-one-third mg-top-30">
                        {/* Using title for now, this will have to be change to path eventually */}
                        {this.state.file ? <VideoPlayer path={this.state.file.title} /> : null}

                        <p>{this.state.file ? (this.state.file.title ? <div><div className="card">
                            <div className="card-content">
                                <b>Titre: </b>{this.state.file.title}
                            </div> </div></div> : <div className="card">
                                <div className="card-content"> This file has no title </div></div>) : null}</p>

                        <br/>

                        <p>{this.state.file ? (this.state.file.description ? <div><div className="card">
                            <div className="card-content">
                            <b>Description: </b>{this.state.file.description}
                            </div> </div></div> : <div className="card">
                                <div className="card-content"> This file has no description </div></div> ) : null}</p>
                    </div>

                    <div className="column mg-top-30">
                        {this.state.version ? <TranscriptionSearch versionId={this.state.version.id} /> : null}
                        {this.state.loading ? 
                            <Loading />
                        : this.state.version && this.state.file && this.state.user ? 
                                <div>
                                    <TranscriptionText
                                        version={this.state.version} 
                                        handleChange={this.handleTranscriptChange} />
                                    <SaveTranscriptionButton
                                        version={this.state.version}
                                        updateVersion={this.updateVersion}
                                        editedTranscription={this.state.editedTranscript}
                                        reviewerId={this.state.file.reviewerId}
                                        userId={this.state.user.id}
                                    />
                                </div>
                        : null}
                        
                    </div>

                    <div className="column mg-top-30">
                    </div>

                </div>
            </div>

        )
    }
}
