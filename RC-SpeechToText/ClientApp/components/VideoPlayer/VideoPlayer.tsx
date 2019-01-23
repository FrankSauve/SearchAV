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
                    <source src={'../assets/Audio/' + this.props.audioFile.name} />
                    <ControlBar autoHide={false}/>
                </Player>
            </div>
        );
    }
}
