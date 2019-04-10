import * as React from 'react';
import auth from '../../Utils/auth';
import axios from "axios";
import { Link } from 'react-router-dom';
import { ErrorModal } from '../Modals/ErrorModal';
import { SuccessModal } from '../Modals/SuccessModal';
import { ConfirmationModal } from '../Modals/ConfirmationModal';
import { SeeTranscriptionModal } from '../Modals/SeeTranscriptionModal';
import { LoadingModal } from '../LoadingModal';

interface State {
    showSaveRevertTranscriptModal: boolean,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    historyTitle: string,
    dateModified: string,
    username: string,
    revertTranscription: boolean,
    showTranscriptionModal: boolean,
    loading: boolean, 
    unauthorized: boolean
}

export class TranscriptionHistoriqueItem extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            showSaveRevertTranscriptModal: false,
            showSuccessModal: false,
            showErrorModal: false,
            historyTitle: this.props.historyTitle,
            dateModified: this.props.dateModified,
            username: this.props.username, 
            revertTranscription: false,
            showTranscriptionModal: false,
            loading: false, 
            unauthorized: false
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

    public showSaveRevertTranscriptModal = () => {
        this.setState({ showSaveRevertTranscriptModal: true });
    }

    public hideSaveRevertTranscriptModal = () => {
        this.setState({ showSaveRevertTranscriptModal: false });
    }

    public showSuccessModal = () => {
        this.setState({ showSuccessModal: true });
    }

    public hideSuccessModal = () => {
        this.setState({ showSuccessModal: false });
    }

    public showErrorModal = () => {
        this.setState({ showErrorModal: true });
    }

    public hideErrorModal = () => {
        this.setState({ showErrorModal: false });
    }

    public revertTranscription = () => {
        //create method to change the Active status of this version to 1 and the last version to 0. 
        this.hideTranscriptionModal();
        this.setState({ 'loading': true });
        var oldTranscript = this.props.activeVersion.transcription
        var newTranscript = this.props.transcription

        if ((oldTranscript == newTranscript || newTranscript == "") && this.props.userId != this.props.reviewerId) {
            this.setState({ 'loading': false });
            this.setState({ 'showErrorModal': true });
        }
        else {
            const formData = new FormData();
            formData.append('newTranscript', newTranscript)

            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                },
            };

            axios.post('/api/Transcription/RevertTranscript/' + this.props.activeVersion.id, formData, config)
                .then(res => {
                    console.log(res);
                    this.props.updateVersion(res.data);
                    this.setState({ 'loading': false });
                    this.showSuccessModal();
                    this.hideSaveRevertTranscriptModal(); 
                    this.props.getAllVersions();
                })
                .catch(err => {
                    if (err.response.status == 401) {
                        this.setState({ 'loading': false });
                        this.setState({ 'unauthorized': true });
                        this.showErrorModal();
                        this.hideSaveRevertTranscriptModal(); 
                    }
                });
        }
    };


    public render() {
        var title = ("Réactiver cette version")
        var button = ("Confirmer")
        return (
            <div>
                <div className="historique-content">
                    <p> {this.state.historyTitle} <a onClick={this.showTranscriptionModal}><i className="fas fa-edit mg-left-10"></i></a></p>
                    <p> {this.state.dateModified}  </p>
                    <p className="historique-username"> <b>{this.props.username}</b></p>
                </div>

                <div>

                    <SeeTranscriptionModal
                        versionId={this.props.version.id}
                        historyTitle={this.state.historyTitle}
                        versionUser={this.props.username}
                        dateModified={this.state.dateModified}
                        transcription={this.props.transcription}
                        showModal={this.state.showTranscriptionModal}
                        hideModal={this.hideTranscriptionModal}
                        revert={this.state.revertTranscription}
                        handleRevertTranscriptionChange={this.handleHistoryChange}
                        onSubmit={this.showSaveRevertTranscriptModal}
                    />

                    <ConfirmationModal
                        showModal={this.state.showSaveRevertTranscriptModal}
                        hideModal={this.hideSaveRevertTranscriptModal}
                        title={title}
                        confirmMessage={"Êtes-vous sûr(e) de vouloir réactiver cette version de la transcription?"}
                        onConfirm={this.revertTranscription}
                        confirmButton={button}
                    />

                    <SuccessModal
                        showModal={this.state.showSuccessModal}
                        hideModal={this.hideSuccessModal}
                        title={title}
                        successMessage={this.props.userId == this.props.reviewerId ? "Révision de la transcription confirmé! Les changements effectués ont été enregistré avec succés." : "Enregistrement de la transcription confirmé! Les changements effectués ont été enregistré avec succés."}
                    />

                    <ErrorModal
                        showModal={this.state.showErrorModal}
                        hideModal={this.hideErrorModal}
                        title={title}
                        errorMessage={this.props.userId == this.props.reviewerId ? "Révision de la transcription annulé! Une erreur est survenue." : "Enregistrement de la transcription annulé! Vous n'avez effectué aucun changements ou vous avez apporté les mêmes modifications."}
                    />

                    <LoadingModal
                        showModal={this.state.loading}
                    />
                </div> 
            </div>
        );
    }
}