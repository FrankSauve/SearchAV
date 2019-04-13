import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { ErrorModal } from './ErrorModal';
import { SuccessModal } from './SuccessModal';
import { ChangeEvent } from 'react';
import { LoadingModal } from '../LoadingModal';
import Loading from '../Loading';

interface State {
    users: any[],
    fileId: any,
    reviewerEmail: string,
    errorMessage: string,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    loading: boolean,
    unauthorized: boolean
}


export class SelectReviewerModal extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            users: [],
            fileId: "",
            reviewerEmail: "",
            errorMessage: "",
            showSuccessModal: false,
            showErrorModal: false,
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        var id = window.location.href.split('/')[window.location.href.split('/').length - 1]; //Getting fileId from url
        this.setState({ fileId: id });
    }

    public addReviewerToFile = () => {
        this.props.hideModal();
        this.setState({ loading: true });

        var fileId = this.state.fileId
        var reviewerEmail = this.state.reviewerEmail

        //Quick fix: Sometimes component is rendered before url changes
        if (fileId == "dashboard" || fileId == "" || fileId == null)
            fileId = window.location.href.split('/')[window.location.href.split('/').length - 1];

        if (fileId != "" && fileId != null && reviewerEmail != null && reviewerEmail != "") {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            };
            axios.get('/api/file/' + fileId + '/' + reviewerEmail, config)
                .then(() => {
                    this.setState({ loading: false });
                    this.showSuccessModal();
                    //this.setState({ reviewerEmail : ""})
                })
                .catch(err => {
                    console.log(err);
                    this.setState({ 'errorMessage': "Le courriel '" + reviewerEmail + "' ne correspond a aucun utilisateur enregistre dans le systeme! Veuillez entrer un autre courriel valide." })
                    this.setState({ loading: false });
                    this.showErrorModal();
                    if (err.response.status == 401) {
                        this.setState({ 'errorMessage': "Vous etes déconnecté! Veuillez vous connecter avant d'envoyer une demande de révision." })
                        this.setState({ 'unauthorized': true });
                    }
                });
        }
        else if (fileId == "" || fileId == null) {
            this.setState({ 'errorMessage': "Envoi de la demande de révision annulée! Une erreur au niveau du fichier est survenu. Veuillez rafraichir la page s'il vous plait." })
            this.setState({ loading: false });
            this.showErrorModal();
        }
        else {
            this.setState({ 'errorMessage': "Envoi de la demande de révision annulée! Vous n'avez pas entrer le courriel du réviseur." })
            this.setState({ loading: false });
            this.showErrorModal();
        }
    };

    public handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        this.setState({ reviewerEmail: e.target.value })
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

                <ErrorModal
                    showModal={this.state.showErrorModal}
                    hideModal={this.hideErrorModal}
                    title="Choisissez un réviseur"
                    errorMessage={this.state.errorMessage}
                />

                <SuccessModal
                    showModal={this.state.showSuccessModal}
                    hideModal={this.hideSuccessModal}
                    title="Choisissez un réviseur"
                    successMessage={"Demande de révision envoyé! Le réviseur (" + this.state.reviewerEmail + ") sera notifié de votre demande dans les quelques secondes a venir."}
                />

                <LoadingModal
                    showModal={this.state.loading}
                />

                <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                    <div className="modal-background"></div>
                    <div className="modal-card modalCard">
                        <div className="modal-container">
                            <header className="modalHeader">
                                <i className="fas fa-clipboard-check fontSize2em mg-right-5"></i><p className="modal-card-title whiteText">Choisissez un reviseur</p>
                                <button className="delete" aria-label="close" onClick={this.props.hideModal}></button>
                            </header>
                            <section className="modalBody">
                                <div className="field">
                                    <div className="control">
                                        <input className="input is-primary" type="email" placeholder="Courriel" onChange={this.handleChange} />
                                    </div>
                                </div>
                            </section>
                            <footer className="modalFooter">
                                <button className="button is-success mg-right-5" onClick={this.addReviewerToFile}>Envoyer la demande</button>
                                <button className="button" onClick={this.props.hideModal}>Annuler</button>
                            </footer>
                        </div>
                    </div>
                </div>
            
            </div>
        );
    }
}
