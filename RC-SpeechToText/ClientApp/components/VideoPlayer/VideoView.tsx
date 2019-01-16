import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

import {VideoInput} from '../Google/VideoInput';
import {VideoPlayer} from './VideoPlayer';

interface State{
    audioFile:any
}

export default class VideoView extends React.Component<any, State>{
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null
        };
    }

    public updateFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]});
        console.log(e.target.files[0]);
    };
    
    public render() {
        return (
            <div className="container">
                <h1 className="title mg-top-30">Prototype Video Player</h1>
                
                <VideoInput
                    onChange={this.updateFile}
                    audioFile={this.state.audioFile}
                />
                <VideoPlayer/>
                
            </div>
        );
    }
}
