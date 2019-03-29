import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import { VideoPlayer } from '../FileView/VideoPlayer';
import { EventHandler, ChangeEvent } from 'react';
import { ErrorModal } from './ErrorModal';
import { SuccessModal } from './SuccessModal';

interface State {
    inputTime: string,
    seekTime: string,
    timeSeconds: number,
    errorMessage: string,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    unauthorized: boolean,
    duration: number
}

export class ThumbnailSelectionModal extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            inputTime: "0",
            seekTime: "00:00:00",
            timeSeconds: 0,
            duration: 0,
            errorMessage: "",
            unauthorized: false,
            showSuccessModal: false,
            showErrorModal: false
        };
    }

    public componentDidMount() {
        this.setState({ duration: this.timeStringToNumber(this.props.file.duration) })
    }

    private timeStringToNumber = (duration: string) => {
        var a = duration.split(':');
        var seconds = (+a[0]) * 60 * 60 + (+a[1]) * 60 + (+a[2]);
        return seconds
    }

    private secondsToTimeString = (seconds: number) => {
        this.setState({ seekTime: new Date(seconds * 1000).toISOString().substr(11, 8) });
    }

    public showSuccessModal = () => {
        this.setState({ showSuccessModal: true });
    }

    public hideSuccessModal = () => {
        this.setState({ showSuccessModal: false });
    }

    public showErrorModal = (description: string) => {
        this.setState({ errorMessage: description });
        this.setState({ showErrorModal: true });
    }

    public hideErrorModal = () => {
        this.setState({ showErrorModal: false });
    }

    public changeThumbnail = (e: ChangeEvent<HTMLInputElement>) => {
        this.setState({ inputTime: e.target.value })

        var inputValue = e.target.value
        var time = parseInt(inputValue)
        var timeToSeek = time / 100
        var inputTimeToVideoTime = timeToSeek * this.state.duration
        this.setState({ timeSeconds: inputTimeToVideoTime })
        this.secondsToTimeString(inputTimeToVideoTime)
    }

    public submit = () => {
        var title = this.props.file.title
        var seek = this.state.timeSeconds

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        };
        axios.get('/api/file/ChangeThumbnail/' + title + '/' + seek, config)
            .then(res => {
                this.showSuccessModal();
            })
            .catch(err => {
                this.showErrorModal("Une erreur est survenu lors du changement du thumbnail")
                this.setState({ 'unauthorized': true });
            });
    }

    public render() {
        return (
            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card modalCard">
                    <div className="modal-container">
                        <header className="modalHeader">
                            <p className="modal-card-title whiteText">Choisir un thumbnail</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            {this.props.file ? <VideoPlayer path={this.props.file.title} controle={false} seekTime={this.state.seekTime} /> : null}
                            <input type="range" step="1" min="0" max="this.state.duration" value={this.state.inputTime} onChange={this.changeThumbnail} />
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-5" onClick={this.submit}>Accepter</button>
                        </footer>
                    </div>
                </div>

                <ErrorModal
                    showModal={this.state.showErrorModal}
                    hideModal={this.hideErrorModal}
                    title="Erreur!"
                    errorMessage={this.state.errorMessage}
                />

                <SuccessModal
                    showModal={this.state.showSuccessModal}
                    hideModal={this.hideSuccessModal}
                    title="Export du transcript"
                    successMessage="Thumbnail changed!"
                />
            </div>
        );
    }
}
