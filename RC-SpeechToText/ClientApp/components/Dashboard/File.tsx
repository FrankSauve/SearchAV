import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

import { FileCard } from './FileCard';

interface State {
   
}

export default class File extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
           
        }
    }

    public render() {   
        return (
            <FileCard
                title={this.props.title}
                flag={this.props.flag}
                username={this.props.username}
                image="assets/speakerIcon.png"
                description={this.props.description}
                transcription={this.props.transcription != null ? this.props.transcription.length > 50 ? this.props.transcription.substring(0, 50) + "..." : this.props.transcription : null}
                date={this.props.dateAdded.substring(0, 10) + " " + this.props.dateAdded.substring(11, 16)}
            />
        )
    }
}