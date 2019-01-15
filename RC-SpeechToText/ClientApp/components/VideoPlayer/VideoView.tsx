import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

import {VideoInput} from '../Google/VideoInput';

interface State{
    audioFile:any,
    loading:boolean
}

export default class VideoView extends React.Component<any, State>{
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null,
            loading: false
        };
    }

    public updateFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]});
        console.log(e.target.files[0]);
    };
    
    public render() {
        return (
            <div className="container">
                <h1 className="title mg-top-30">Video Player</h1>
                
                <VideoInput
                    onChange={this.updateFile}
                    audioFile={this.state.audioFile}
                />
                {
                    /*TODO: Add Video Player here*/
                }
                
            </div>
        );
    }
}
