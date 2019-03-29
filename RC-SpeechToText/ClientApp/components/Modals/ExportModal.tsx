import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import { ErrorModal } from './ErrorModal';
import { SuccessModal } from './SuccessModal';
import { EventHandler, ChangeEvent } from 'react';
import { LoadingModal } from '../LoadingModal';

interface State {
    fileId: AAGUID,
    errorMessage: string,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    hideBurnSubtitleRadioButton: boolean,
    burnVideoInput: boolean,
    loading: boolean,
    unauthorized: boolean,
    documentOption: string
}

export class ExportModal extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            fileId: "",
            errorMessage: "",
            showSuccessModal: false,
            showErrorModal: false,
            hideBurnSubtitleRadioButton: true,
            burnVideoInput: false,
            loading: false,
            unauthorized: false,
            documentOption: ""
        }
    }

    public handleOptionChange = (e: ChangeEvent<HTMLSelectElement>) => {
        this.setState({ documentOption: e.target.value });
        if (e.target.value == "video") {
            this.setState({ hideBurnSubtitleRadioButton: false });
        }
        else {
            this.setState({ hideBurnSubtitleRadioButton: true });
        }
    }

    public handleBurnVideoChange = (e: ChangeEvent<HTMLInputElement>) => {
        this.setState({ burnVideoInput: e.target.checked });
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

    // Called when the component gets rendered
    public componentDidMount() {
        var id = window.location.href.split('/')[window.location.href.split('/').length - 1]; //Getting fileId from url
        this.setState({ fileId: id });
    }

    public saveDocument = () => {
        this.setState({ loading: true });

        var fileId = this.state.fileId;
        var exportSelected = this.state.documentOption;

        if (this.state.burnVideoInput && this.state.documentOption == "video")
            exportSelected += "burn"

        if (fileId != "" && exportSelected != "" && exportSelected != "0") {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            };
            axios.get('/api/transcription/downloadtranscript/' + fileId + '/' + exportSelected, config)
                .then(res => {
                    this.setState({ loading: false });
                    this.showSuccessModal();
                })
                .catch(err => {
                    this.setState({ loading: false });
                    this.showErrorModal("Une erreur est survenu lors de l'export du fichier")
                    this.setState({ 'unauthorized': true });
                });
        } else {
            if (fileId == "")
                this.setState({ loading: false });
                this.showErrorModal("Une erreur est survenu lors de la selection du fichier");
            if (exportSelected == "" || exportSelected == "0")
                this.setState({ loading: false });
                this.showErrorModal("Choisier le type de document dont vous voulez exporter");
        }
    }

    public render() {
        return (
            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >

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
                    successMessage="Document exporté!"
                />

                <LoadingModal
                    showModal={this.state.loading}
                />

                <div className="modal-background"></div>
                <div className="modal-card modalCard">
                    <div className="modal-container">
                        <header className="modalHeader">
                            <i className="fas fa-file-export fontSize2em mg-right-5"></i><p className="modal-card-title whiteText">{this.props.title}</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal} ></button>
                    </header>
                        <section className="modalBody">
                            <div className="modalSection">
                                <p className="padding-bottom-10">Format de fichier :</p>
                            </div>
                            <div className="padding-bottom-10">
                                <select className="exportModalDropdown" onChange={this.handleOptionChange}>
                                    <option value="0">Choisissez un format</option>   
                                    <option value="doc">.DOC</option>
                                    <option value="srt">.SRT</option>
                                    <option value="googleDoc">Google Doc</option>
                                    <option value="video">Video</option>
                                 </select>
                            </div>
                            <input type="checkbox" value="burn" className="mg-right-5" onChange={this.handleBurnVideoChange} hidden={this.state.hideBurnSubtitleRadioButton}></input><span hidden={this.state.hideBurnSubtitleRadioButton}>Incrustrer les sous-titres sur la vidéo</span>
                    </section>
                        <footer className="modalFooter">
                            <button className="button is-success pull-right" onClick={this.saveDocument}>Confirmer</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
