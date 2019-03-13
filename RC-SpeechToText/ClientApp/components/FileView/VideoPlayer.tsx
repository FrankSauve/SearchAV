import * as React from 'react';
import ReactPlayer from 'react-player';

interface State {
    player: any
}

export class VideoPlayer extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            player: null
        }
    }

    public changeTime = () => {
        this.state.player.seekTo(11);
    }
    
    ref = (player: any) => {
        this.setState({player: player});
      }
    public render() {
        return (
            <div>
                <ReactPlayer
                    ref={this.ref}
                    url={'../assets/Audio/' + this.props.path}
                    playing={false}
                    controls={true}
                    width='100%'
                    height='100%'
                />
                <button onClick={this.changeTime}>Change time yo</button>
            </div>
        );
    }
}
