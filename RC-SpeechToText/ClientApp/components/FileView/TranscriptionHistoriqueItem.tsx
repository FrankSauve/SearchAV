import * as React from 'react';
import { Link } from 'react-router-dom';
import { SeeTranscriptionModal } from '../Modals/SeeTranscriptionModal';

interface State {
    historyTitle: string,
    dateModified: string,
    username: any,
    revertTranscription: boolean,
    showTranscriptionModal: boolean
}

export class TranscriptionHistoriqueItem extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            historyTitle: this.props.title,
            dateModified: this.props.description,
            username: this.props.username, 
            revertTranscription: false,
            showTranscriptionModal: false
        }
    }

    public showTranscriptionModal = () => {
        this.setState({ showTranscriptionModal: true });
    }

    public hideTranscriptionModal = () => {
        this.setState({ showTranscriptionModal: false });
    }

    public handleHistoryChange = (event: any) => {
        this.setState({ revertTranscription: event.target.value });
    };

    public revertTranscription = () => {
        //create method to change the Active status of this version to 1 and the last version to 0. 
    };

    public render() {
        return (
            <div>
                <div className="historique-content">
                    <p> {this.state.historyTitle} </p>
                    <p> {this.state.dateModified} <a onClick={this.showTranscriptionModal}><i className="fas fa-edit mg-left-5"></i></a> </p>
                    <p className="historique-username"> <b>{this.state.username}</b></p>
                </div>

                <div>
                    <SeeTranscriptionModal
                        versionId={this.props.version.id}
                        historyTitle={this.state.historyTitle}
                        versionUser={this.state.username}
                        dateModified={this.state.dateModified}
                        transcription={this.props.transcription}
                        showModal={this.state.showTranscriptionModal}
                        hideModal={this.hideTranscriptionModal}
                        revert={this.state.revertTranscription}
                        handleRevertTranscriptionChange={this.handleHistoryChange}
                        onSubmit={this.revertTranscription}
                    />
                </div> 
            </div>
        );
    }
}