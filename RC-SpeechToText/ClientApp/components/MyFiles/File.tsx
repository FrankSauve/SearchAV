import * as React from 'react';
import axios from 'axios';

import { FileCard } from './FileCard';

interface State {
    fileId: number,
    title: string,
    flag: string,
    filePath: string, 
    transcription: string,
    dateAdded: string,
    type: string, 
    userId: number
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
            userId: this.props.userId
        }
    }

    

    public render() {   
        return (
            <FileCard
                title={this.state.title}
                flag={this.state.flag}
                image="assets/speakerIcon.png"
                transcription={this.state.transcription != null ? this.state.transcription.length > 50 ? this.state.transcription.substring(0, 50) + "..." : this.state.transcription : null}
                date={this.state.dateAdded.substring(0, 10) + " " + this.state.dateAdded.substring(11, 16)}
            />
        )
    }
}