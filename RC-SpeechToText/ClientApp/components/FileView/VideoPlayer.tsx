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
                    <source src={'../assets/Audio/' + this.props.path} />
                    <ControlBar autoHide={false}/>
                </Player>
            </div>
        );
    }
}
