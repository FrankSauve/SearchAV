import * as React from 'react';



import {Player, ControlBar} from 'video-react';
import {
    Button, Form, FormGroup,
    Label, Input, Col
} from 'reactstrap';

export class VideoPlayer extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    

    public render() {
        return (
            <div>
                <Player
                    ref="player"
                    autoPlay
                >
                    <source src={'http://media.w3.org/2010/05/sintel/trailer.mp4'} />
                    <ControlBar autoHide={false}/>
                </Player>
            </div>
        );
    }
}
