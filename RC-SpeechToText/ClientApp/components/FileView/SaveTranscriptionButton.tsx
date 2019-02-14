import * as React from 'react';
import axios from "axios";
import auth from '../../Utils/auth';
import { ErrorModal } from '../Modals/ErrorModal';
import { SuccessModal } from '../Modals/SuccessModal';
import { ConfirmationModal } from '../Modals/ConfirmationModal';

interface State {
    showSaveTranscriptModal: boolean,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    unauthorized: boolean
}

export class SaveTranscriptionButton extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            showSaveTranscriptModal: false,
            showSuccessModal: false,
            showErrorModal: false,
            unauthorized: false
        }

    }

    public saveEditedTranscription = () => {
        
        var oldTranscript = this.props.version.transcription
        var newTranscript = this.props.editedTranscription

        if (oldTranscript == newTranscript || newTranscript == "") {
            this.hideSaveTranscriptModal();
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

            axios.post('/api/SaveEditedTranscript/SaveEditedTranscript/' + this.props.version.id, formData, config)
                .then(res => {
                    console.log(res);
                    this.props.updateVersion(res.data);
                    this.hideSaveTranscriptModal();
                    this.showSuccessModal();
                })
                .catch(err => {
                    if (err.response.status == 401) {
                        this.setState({ 'unauthorized': true });
                    }
                });
        }

    }

    public showSaveTranscriptModal = () => {
        this.setState({ showSaveTranscriptModal: true });
    }

    public hideSaveTranscriptModal = () => {
        this.setState({ showSaveTranscriptModal: false });
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

    public render() {

        return (
            <div>

                <ConfirmationModal
                    showModal={this.state.showSaveTranscriptModal}
                    hideModal={this.hideSaveTranscriptModal}
                    title="Sauvegarder la transcription"
                    confirmMessage="Êtes-vous sûr(e) de vouloir enregistrer les changements effectués à la transcription?"
                    onConfirm={this.saveEditedTranscription}
                    confirmButton="Enregistrer"
                />

                <SuccessModal
                    showModal={this.state.showSuccessModal}
                    hideModal={this.hideSuccessModal}
                    title="Sauvegarder la transcription"
                    successMessage="Enregistrement confirmé! Les changements effectués ont été enregistré avec succés."
                />

                <ErrorModal
                    showModal={this.state.showErrorModal}
                    hideModal={this.hideErrorModal}
                    title="Sauvegarder la transcription"
                    errorMessage="Enregistrement annulé! Vous n'avez effectué aucun changements ou vous avez apporté les mêmes modifications."
                />

                <a className="button is-link mg-top-10" onClick={this.showSaveTranscriptModal}>Enregistrer</a>
            </div>
        );
    }
}
