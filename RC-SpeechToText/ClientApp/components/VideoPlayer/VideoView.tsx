import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

import {VideoPlayer} from './VideoPlayer';


export class VideoView extends React.Component<any>{
    constructor(props: any) {
        super(props);
    }
    
    public render() {
        return (
            <div className="container">

                {(this.props.showVideo) && (this.props.audioFile != null) ?
                    <div>
                        <h1 className="title mg-top-30">Prototype Video Player</h1>


                        <VideoPlayer
                            audioFile={this.props.audioFile}
                        />
                    </div>
                    : ''}
                
            </div>
        );
    }
}
