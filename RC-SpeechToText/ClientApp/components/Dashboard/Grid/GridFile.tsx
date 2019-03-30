import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

import { GridFileCard } from './GridFileCard';

interface State {
    transcription: string,
    unauthorized: boolean
}

export default class GridFile extends React.Component<any, State> {

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

        axios.get('/api/version/GetActivebyFileId/' + this.props.file.id, config)
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
            <GridFileCard
                file={this.props.file}
                title={this.props.title}
                description={this.props.description != null ? this.props.description.length > 100 ? this.props.description.substring(0,100) + "..." : this.props.description : null}
                flag={this.props.flag}
                username={this.props.username}
                image={this.props.type == "Audio" ? 'assets/audioIcon.png' : this.props.thumbnailPath}
                duration={this.props.duration}
                transcription={this.state.transcription != null ? this.state.transcription.length > 100 ? this.state.transcription.substring(0, 100) + "..." : this.state.transcription : null}
                date={this.props.dateAdded.substring(0, 10) + " " + this.props.dateAdded.substring(11, 16)}
                updateFiles={this.props.updateFiles}
            />
        )
    }
}