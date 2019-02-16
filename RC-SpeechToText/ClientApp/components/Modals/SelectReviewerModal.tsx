import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { ErrorModal } from './ErrorModal';
import { SuccessModal } from './SuccessModal';

interface State {
    users: any[],
    fileId: any,
    reviewerId: number,
    reviewerName: string,
    reviewerEmail: string,
    errorMessage: string,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    unauthorized: boolean
}


export class SelectReviewerModal extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            users: [],
            fileId: 0,
            reviewerId: 0,
            reviewerName: "",
            reviewerEmail: "",
            errorMessage: "",
            showSuccessModal: false,
            showErrorModal: false,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        var id = window.location.href.split('/')[window.location.href.split('/').length - 1]; //Getting fileId from url
        this.setState({ fileId: id });
        this.getAllUsers();
    }

    public getAllUsers = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/user/GetAllUsers', config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'users': res.data });
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public addReviewerToFile = () => {

        var fileId = this.state.fileId
        var reviewerId = this.state.reviewerId

        if ((fileId != "" && fileId != 0) && reviewerId != 0) {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            }
            axios.post('/api/file/AddReviewer/' + fileId + '/' + reviewerId, config)
                .then(res => {
                    console.log(res.data);

                    axios.get('/api/user/getUserName/' + reviewerId, config)
                        .then(res => {
                            console.log(res);
                            this.setState({ 'reviewerName': res.data.name })
                            this.setState({ 'reviewerEmail': res.data.email })
                        })
                        .catch(err => {
                            if (err.response.status == 401) {
                                this.setState({ 'unauthorized': true });
                            }
                        });

                    this.props.hideModal();
                    this.showSuccessModal();
                })
                .catch(err => {
                    console.log(err);
                    this.setState({ 'errorMessage': "Vous n'etes pas connecté! Veuillez vous connecter s'il vous plait."})
                    this.props.hideModal();
                    this.showErrorModal();
                    if (err.response.status == 401) {
                        this.setState({ 'unauthorized': true });
                    }
                });
        }
        else if (fileId == "" || fileId == 0){
            this.setState({ 'errorMessage': "Envoi de demande de révision annulé! Une erreur au niveau du fichier est survenu. Veuillez rafraichir la page s'il vous plait." })
            this.props.hideModal();
            this.showErrorModal();
        }
        else {
            this.setState({ 'errorMessage': "Envoi de demande de révision annulé! Vous n'avez selectionné aucun réviseur." })
            this.props.hideModal();
            this.showErrorModal();
        }
    };

    public handleChange = (event: any) => {
        this.setState({ reviewerId: event.target.value });
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
                    successMessage={`Demande de révision envoyé! ${this.state.reviewerName} (${this.state.reviewerEmail}) sera notifié de votre demande dans les quelques secondes a venir.`}
/>

            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card">
                    <header className="modal-card-head">
                        <p className="modal-card-title">Choisissez un reviseur</p>
                        <button className="delete" aria-label="close" onClick={this.props.hideModal}></button>
                    </header>
                        <section className="modal-card-body">
                            <div className="select is-multiple">
                            <select multiple size={8} onChange={this.handleChange}>
                                {this.state.users.map((user) => {
                                    {
                                        //Includes current user's name for testing purposes
                                    }
                                    const listUsers = <option value={user.id}>{user.name} | {user.email}</option>
                                    return (
                                        listUsers
                                    )
                                })}
                            </select>
                        </div>
                    </section>
                    <footer className="modal-card-foot">
                        <button className="button is-success" onClick={this.addReviewerToFile}>Envoyer la demande</button>
                        <button className="button" onClick={this.props.hideModal}>Annuler</button>
                    </footer>
                </div>
                </div>
            </div>
        );
    }
}
