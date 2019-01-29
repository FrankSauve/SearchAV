import * as React from 'react';
import axios from 'axios';

import { FileCard } from './FileCard';

interface State {
    fileId: number,
    title: string,
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
                image="assets/speakerIcon.png"
                transcription={this.state.transcription != null ? this.state.transcription.length > 100 ? this.state.transcription.substring(0, 100) : this.state.transcription : null}
                date={this.state.dateAdded.substring(0,10)}
            />
        )
    }
}