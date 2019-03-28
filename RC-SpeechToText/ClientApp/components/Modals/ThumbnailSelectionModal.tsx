import * as React from 'react';
import { VideoPlayer } from '../FileView/VideoPlayer';
import { EventHandler, ChangeEvent } from 'react';

interface State {
    inputTime: string,
    seekTime: string,
    duration: number
}

export class ThumbnailSelectionModal extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            inputTime: "0",
            seekTime: "0",
            duration: 0
        };
    }

    public componentDidMount() {
        this.timeStringToNumber(this.props.file.duration)
    }

    private timeStringToNumber = (duration: string) => {
        var a = duration.split(':');
        var seconds = (+a[0]) * 60 * 60 + (+a[1]) * 60 + (parseFloat(a[2]));
        this.setState({ duration: seconds })
    }

    private secondsToTimeString = (seconds: number) => {
        this.setState({ seekTime: new Date(seconds * 1000).toISOString().substr(11, 8) });
    }

    public changeThumbnail = (e: ChangeEvent<HTMLInputElement>) => {
        this.setState({ inputTime: e.target.value })

        var inputValue = e.target.value
        var time = parseInt(inputValue)
        var timeToSeek = time / 100
        var inputTimeToVideoTime = timeToSeek * this.state.duration
        this.secondsToTimeString(inputTimeToVideoTime)
    }

    public render() {
        return (
            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card modalCard">
                    <div className="modal-container">
                        <header className="modalHeader">
                            <p className="modal-card-title whiteText">Choose a thumbnail</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            {this.props.file ? <VideoPlayer path={this.props.file.title} controle={false} seekTime={this.state.seekTime} /> : null}
                            <input type="range" step="1" min="0" max="this.state.duration" value={this.state.inputTime} onChange={this.changeThumbnail} />
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-5" onClick={this.props.onSubmit}>Accepter</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
