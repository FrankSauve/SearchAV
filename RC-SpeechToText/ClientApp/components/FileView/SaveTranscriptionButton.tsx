import * as React from 'react';
import axios from "axios";
import auth from '../../Utils/auth';

interface State {
    showSaveTranscriptModal: boolean,
    showErrorModal: boolean,
    unauthorized: boolean
}

export class SaveTranscriptionButton extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {

            showSaveTranscriptModal: false,
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

    public showErrorModalModal = () => {
        this.setState({ showErrorModal: true });
    }

    public hideErrorModalModal = () => {
        this.setState({ showErrorModal: false });
    }

    public render() {

        return (
            <div>

                <div className={`modal ${this.state.showSaveTranscriptModal ? "is-active" : null}`} >
                    <div className="modal-background"></div>
                    <div className="modal-card">
                        <header className="modal-card-head">
                            <p className="modal-card-title">Sauvegarder la transcription</p>
                            <button className="delete" aria-label="close" onClick={this.hideSaveTranscriptModal}></button>
                        </header>
                        <section className="modal-card-body">
                            <p>Etes-vous sur(e) d'enregistrer les changements effectues a la transcription?</p>
                        </section>
                        <footer className="modal-card-foot">
                            <button className="button is-success" onClick={this.saveEditedTranscription}>Enregistrer</button>
                            <button className="button" onClick={this.hideSaveTranscriptModal}>Annuler</button>
                        </footer>
                    </div>
                </div>

                <div className={`modal ${this.state.showErrorModal ? "is-active" : null}`} >
                    <div className="modal-background"></div>
                    <div className="modal-card">
                        <header className="modal-card-head">
                            <p className="modal-card-title">Sauvegarder la transcription</p>
                            <button className="delete" aria-label="close" onClick={this.hideErrorModalModal}></button>
                        </header>
                        <section className="modal-card-body">
                            <p className="has-text-danger">Enregistrement annule! Vous n'avez effectue aucun changements ou vous avez apporte les memes modifications.</p>
                        </section>
                        <footer className="modal-card-foot">
                        </footer>
                    </div>
                </div>

                <a className="button is-danger" onClick={this.showSaveTranscriptModal}>Enregistrer</a>
            </div>
        );
    }
}
