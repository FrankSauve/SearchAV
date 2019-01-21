import * as React from 'react';



import {Player, ControlBar} from 'video-react';


export class VideoPlayer extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    

    public render() {
        return (
            <div>
                <Player
                    ref="player"
                    autoplay
                >
                    {(this.props.audioFile != null) ? 
                        console.log('audioFile: '+this.props.audioFile.name):''}
                    <source src={'D:\\Concordia University\\Capstone\\FL.mp4'} />
                    <ControlBar autoHide={false}/>
                </Player>
            </div>
        );
    }
}
