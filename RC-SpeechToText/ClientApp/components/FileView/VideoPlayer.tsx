import * as React from 'react';
import ReactPlayer from 'react-player';

interface State {
    player: any,
    playing: boolean,
    controls: boolean
}

export class VideoPlayer extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            player: null,
            playing: false,
            controls: this.props.controls
        }

    }
    
    handleKeyDown = (event : any) => {

        if(event.altKey) { // alt key
            if(event.keyCode == 66){ // b key
                console.log("GetCurrentTimeWork START?: "+this.state.player.getCurrentTime());
                this.props.handleHighlightPosition(this.state.player.getCurrentTime());
            }
            else if(event.keyCode == 78){ // n key
                console.log("GetCurrentTimeWork END?: "+this.state.player.getCurrentTime());
                this.props.handleHighlightPosition(null);
            }
        }
    };
    
    localSeek = (n: any) =>{
        let seekVal = this.state.player.getCurrentTime()+n;
        if (seekVal < 0)
            seekVal = 0;
        if(seekVal >= this.state.player.getDuration())
            seekVal = this.state.player.getDuration() - 1;
        this.state.player.seekTo(seekVal);
    };

    componentDidMount() {
        document.addEventListener("keydown", this.handleKeyDown);
    }

    componentWillUnmount() {
        document.removeEventListener("keydown", this.handleKeyDown);
    }
    
    componentDidUpdate(prevProps : any, prevState : any) {
        // only call for the change in time if the data has changed
        if (prevProps.seekTime !== this.props.seekTime) {
            this.changeTime();
        }
    }

    public changeTime = () => {
        let a = this.props.seekTime.split(':');
        let seconds = (+a[0]) * 60 * 60 + (+a[1]) * 60 + (parseFloat(a[2]));
        this.state.player.seekTo(seconds);
    };
    
    ref = (player: any) => {
        this.setState({player: player});
    };
    
    public render() {
        return (
            <div className="video-player">
                <ReactPlayer
                    ref={this.ref}
                    url={this.props.path}
                    playing={this.state.playing}
                    controls={this.state.controls}
                    width='100%'
                    height='100%'
                />
            </div>
        );
    }
}
