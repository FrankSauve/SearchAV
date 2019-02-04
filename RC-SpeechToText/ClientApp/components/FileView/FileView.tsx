import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { TranscriptionText } from './TranscriptionText';
import { VideoPlayer } from './VideoPlayer';

interface State {
    fileId: number,
    version: any,
    file: any,
    unauthorized: boolean,
    fileTitle: String
}

export default class FileView extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            fileId: this.props.match.params.id,
            version: null,
            file: null,
            unauthorized: false,
            fileTitle: ""
        }

        this.handleChange = this.handleChange.bind(this);
        this.saveTitleChange = this.saveTitleChange.bind(this);
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

    handleChange(event: React.FormEvent<HTMLTextAreaElement>) {
        var safeSearchTypeValue: string = event.currentTarget.value;

        this.setState({ fileTitle: safeSearchTypeValue });
    }

    saveTitleChange() {
        var oldTitle = this.state.file.title
        var newTitle = this.state.fileTitle

        const formData = new FormData();
        formData.append("fileId", this.state.fileId + '')
        formData.append("newTitle", newTitle + '')

        if (oldTitle != newTitle) {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            }

            axios.put('/api/file/ModifyTitle', formData, config)
                .then(res => {
                    this.setState({ file: res.data });
                })
                .catch(err => {
                    if (err.response.status == 401) {
                        this.setState({ 'unauthorized': true });
                    }
                });
        }
    }

    render() {
        return (
            <div className="container">
                <div className="columns">
                    <div className="column is-one-third mg-top-30">
                        {/* Using title for now, this will have to be change to path eventually */}
                        {this.state.file ? <VideoPlayer path={this.state.file.title}/> : null} 
                    </div>
                    <div className="column mg-top-30">
                        {this.state.file ?
                            <textarea
                                className="title-area"
                                rows={1}
                                defaultValue={this.state.file.title}
                                onChange={this.handleChange}>
                            </textarea> :
                            null
                        }
                        <button className="save-title-button" onClick={this.saveTitleChange}>
                            Save
                        </button>
                        {this.state.version ? <TranscriptionText text={this.state.version.transcription} /> : null}
                    </div>
                </div>
            </div>
        )
    }
}
