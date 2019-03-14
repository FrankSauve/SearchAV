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

    componentDidUpdate(prevProps : any, prevState : any) {
        // only call for the change in time if the data has changed
        if (prevProps.seekTime !== this.props.seekTime) {
            this.changeTime();
        }
    }

    public changeTime = () => {
        var hrs = parseInt(this.props.seekTime.substring(0, 1));
        var mins = parseInt(this.props.seekTime.substring(2, 2) + hrs * 60.0);
        var secs = parseFloat(this.props.seekTime.substring(5) + mins * 60.0);
        this.state.player.seekTo(secs);
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
            </div>
        );
    }
}
