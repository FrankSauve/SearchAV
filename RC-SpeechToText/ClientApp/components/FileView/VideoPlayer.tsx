import * as React from 'react';
import ReactPlayer from 'react-player';

interface State {
    player: any,
    playing: boolean,
    
}

export class VideoPlayer extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            player: null,
            playing: false
        }

    }

    handleKeyDown = (event: any) => {
        //console.log("isPlaying? " + this.state.isPlaying);
        
        if(event.ctrlKey) { //ctrl key
            if (event.keyCode == 37) { //left arrow
                this.localSeek(-5);
            }
            else if (event.keyCode == 40 || event.keyCode == 32) { //down arrow or space
                this.playPause();
            }
            else if (event.keyCode == 39) { //right arrow
                this.localSeek(5);
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
    
    playPause = () =>{
        this.setState({playing: !this.state.playing})
    };
    
    onPlay = () =>{
        this.setState({playing:true})
    };
    onPause = () =>{
        this.setState({playing:false})
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
        let hrs = parseInt(this.props.seekTime.substring(0, 1));
        let mins = parseInt(this.props.seekTime.substring(2, 2) + hrs * 60.0);
        let secs = parseFloat(this.props.seekTime.substring(5) + mins * 60.0);
        this.state.player.seekTo(secs);
    };
    
    ref = (player: any) => {
        this.setState({player: player});
    };
    public render() {
        return (
            <div className="video-player">
                <ReactPlayer
                    ref={this.ref}
                    url={'../assets/Audio/' + this.props.path}
                    playing={this.state.playing}
                    onPause={this.onPause}
                    onPlay={this.onPlay}
                    controls={true}
                    width='100%'
                    height='100%'
                />
            </div>
        );
    }
}
