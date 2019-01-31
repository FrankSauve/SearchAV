import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

import { FileCard } from './FileCard';

interface State {
    transcription: string,
    unauthorized: boolean
}

export default class File extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            transcription: "",
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getActiveTranscription();
    }

    public getActiveTranscription = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/version/GetActivebyFileId/' + this.props.fileId, config)
            .then(res => {
                console.log(res);
                this.setState({ 'transcription': res.data.transcription })
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {   
        return (
            <FileCard
                title={this.props.title}
                flag={this.props.flag}
                username={this.props.username}
                image="assets/speakerIcon.png"
                transcription={this.state.transcription != null ? this.state.transcription.length > 100 ? this.state.transcription.substring(0, 100) : this.state.transcription : null}
                date={this.props.dateAdded.substring(0, 10) + " " + this.props.dateAdded.substring(11, 16)}
            />
        )
    }
}