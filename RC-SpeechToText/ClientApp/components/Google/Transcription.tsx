import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

import {VideoInput} from './VideoInput';

interface State{
    audioFile:any
}

export default class Transcription extends React.Component<any, State>{
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null
        };
    }

    public onAddAudioFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]});
        console.log(e.target.files[0]); //TODO: Remove this line
    };

    public render() {
        
        
        
        return (
            <div className="container">
                <h1 className="title mg-top-30">Transcription</h1>
                <VideoInput onChange={this.onAddAudioFile}/>
            </div>
        );
    }
}
