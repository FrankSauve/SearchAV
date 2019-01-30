import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

import { FileCard } from './FileCard';

interface State {
    fileId: number,
    title: string,
    flag: string,
    filePath: string, 
    transcription: string,
    dateAdded: string,
    type: string, 
    username: string,
    userId: number,
    unauthorized: boolean
}

export default class File extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            fileId: this.props.fileId,
            title: this.props.title,
            flag: this.props.flag,
            filePath: this.props.filePath,
            transcription: this.props.transcription,
            dateAdded: this.props.dateAdded,
            type: this.props.type,
            username:"",
            userId: this.props.userId,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getUsername();
    }

    public getUsername = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/user/getUserName/' + this.state.userId, config)
            .then(res => {
                console.log(res);
                this.setState({ 'username': res.data.name })
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
                title={this.state.title}
                flag={this.state.flag}
                username={this.state.username}
                image="assets/speakerIcon.png"
                transcription={this.state.transcription != null ? this.state.transcription.length > 50 ? this.state.transcription.substring(0, 50) + "..." : this.state.transcription : null}
                date={this.state.dateAdded.substring(0, 10) + " " + this.state.dateAdded.substring(11, 16)}
            />
        )
    }
}