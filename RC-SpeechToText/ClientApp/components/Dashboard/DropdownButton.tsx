﻿import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { ModifyTitleModal } from '../Modals/ModifyTitleModal';
import { ModifyDescriptionModal } from '../Modals/ModifyDescriptionModal';
import { SuccessModal } from '../Modals/SuccessModal';
import { ErrorModal } from '../Modals/ErrorModal';

interface State {
    fileId: number,
    title: string,
    description: string,
    modifiedTitle: string,
    newDescription: string,
    flag: string,
    showDropdown: boolean,
    showTitleModal: boolean,
    showDescriptionModal: boolean,
    showErrorModal: boolean,
    showSuccessModal: boolean,
    modalTitle: string,
    errorMessage: string,
    successMessage: string,
    unauthorized: boolean
}

export class DropdownButton extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            fileId: this.props.fileId,
            title: this.props.title,
            description: this.props.description,
            modifiedTitle: "",
            newDescription: "",
            flag: this.props.flag,
            showDropdown: false,
            showTitleModal: false,
            showDescriptionModal: false,
            showErrorModal: false,
            showSuccessModal: false,
            modalTitle: "",
            errorMessage: "",
            successMessage: "",
            unauthorized: false
        }
    }

    //Will update if props change
    public componentDidUpdate(prevProps: any) {
        if (this.props.flag !== prevProps.flag) {
            this.setState({ 'flag': this.props.flag });
        }
    }

    // Add event listener for a click anywhere in the page
    componentDidMount() {
        document.addEventListener('mouseup', this.hideDropdown);
    }
    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.hideDropdown);
    }

    public deleteWords = () => {

        console.log("Deleting words");

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.delete('/api/word/DeleteByFileId/' + this.state.fileId, config)
            .then(res => {
                console.log(res.status);
                this.deleteVersion();
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public deleteVersion = () => {

        console.log(this.props.fileId);

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }

        axios.delete('/api/version/DeleteFileVersions/' + this.state.fileId, config)
            .then(res => {
                console.log(res.data);
                this.deleteFile();
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public deleteFile = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.delete('/api/file/Delete/' + this.state.fileId, config)
            .then(res => {
                console.log(res.data);
                alert("File deleted");
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public saveTitleChange = () => {

        var oldTitle = this.state.title
        var newTitle = this.state.modifiedTitle

        if (oldTitle != newTitle && newTitle != "") {
            const formData = new FormData();
            formData.append("newTitle", newTitle)

            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            }

            axios.put('/api/file/ModifyTitle/' + this.state.fileId, formData, config)
                .then(res => {
                    this.setState({ title: this.state.modifiedTitle });
                    this.hideTitleModal();
                    this.showSuccessModal("Modifier le titre", "Enregistrement du titre confirmé! Les changements effectués ont été enregistré avec succès.");
                })
                .catch(err => {
                    console.log(err);
                    if (err.response.status == 401) {
                        this.showErrorModal("Modifier le titre", "Veuillez vous connecter avant de modifier le titre.");
                        this.setState({ 'unauthorized': true });
                    }
                    else if (err.response.status == 400) {
                        this.showErrorModal("Modifier le titre", err.response.data);
                    }
                });
        }
        else {
            this.showErrorModal("Modifier le titre", "Enregistrement du titre annulé! Vous n'avez effectué aucun changements ou vous avez apporté les mêmes modifications.");
        }


    }

    public saveDescription = () => {

        var oldDescription = this.state.description
        var newDescription = this.state.newDescription

        var modalTitle = (this.state.description && this.state.description != "" ? "Modifier la description" : "Ajouter une description")

        const formData = new FormData();
        formData.append("newDescription", newDescription)

        if (oldDescription != newDescription && newDescription != "") {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            }

            axios.put('/api/file/saveDescription/' + this.state.fileId, formData, config)
                .then(res => {
                    this.setState({ description: this.state.newDescription });
                    this.hideDescriptionModal();
                    this.showSuccessModal(modalTitle, "Enregistrement de la description confirmé! Les changements effectués ont été enregistré avec succès.");
                })
                .catch(err => {
                    if (err.response.status == 401) {
                        this.showErrorModal(modalTitle, "Veuillez vous connecter avant de modifier la description.");
                        this.setState({ 'unauthorized': true });
                    }
                });
        }
        else {
            this.showErrorModal(modalTitle, "Enregistrement de la description annulé! Vous n'avez effectué aucun changements ou vous avez apporté les mêmes modifications.");
        }
    }

    rawToWhiteSpace(text: string) {
        return text.replace(/<br\s*[\/]?>/gi, " ");
    }

    public handleTitleChange = (event: any) => {
        this.setState({ modifiedTitle: event.target.value });
    }

    public handleDescriptionChange = (event: any) => {
        this.setState({ newDescription: event.target.value });
    }

    public showDropdown = () => {
        this.setState({ showDropdown: true });
    }

    public hideDropdown = () => {
        this.setState({ showDropdown: false });
    }

    public showTitleModal = () => {
        this.setState({ showTitleModal: true });
    }

    public hideTitleModal = () => {
        this.setState({ showTitleModal: false });
    }

    public showDescriptionModal = () => {
        this.setState({ showDescriptionModal: true });
    }

    public hideDescriptionModal = () => {
        this.setState({ showDescriptionModal: false });
    }

    public showSuccessModal = (title: string, description: string) => {
        this.setState({ successMessage: description });
        this.setState({ modalTitle: title });
        this.setState({ showSuccessModal: true });
    }

    public showErrorModal = (title: string, description: string) => {
        this.setState({ errorMessage: description });
        this.setState({ modalTitle: title });
        this.setState({ showErrorModal: true });
    }

    public hideSuccessModal = () => {
        this.setState({ showSuccessModal: false });
    }

    public hideErrorModal = () => {
        this.setState({ showErrorModal: false });
    }

    public render() {
        return (
            <div>
                <div className={`dropdown is-right ${this.state.showDropdown ? "is-active" : null}`} >
                    <div className="dropdown-trigger">
                        <div className="is-black" aria-haspopup="true" aria-controls="dropdown-menu4" onClick={this.showDropdown}>
                            <i className="fas fa-ellipsis-v "></i>
                        </div>
                    </div>
                    <div className="dropdown-menu" id="dropdown-menu4" role="menu">
                        <div className="dropdown-content">
                            <a className="dropdown-item" onClick={this.showTitleModal}>
                                Modifier le titre
                                    </a>
                            <a className="dropdown-item" onClick={this.deleteWords}>
                                Effacer le fichier
                                    </a>
                            {this.props.description ? <a className="dropdown-item" onClick={this.showDescriptionModal}>
                                Modifier la description
                                    </a> : <a className="dropdown-item" onClick={this.showDescriptionModal}>
                                    Ajouter une description
                                    </a>}
                        </div>
                    </div>
                </div>

                <ModifyTitleModal
                    showModal={this.state.showTitleModal}
                    hideModal={this.hideTitleModal}
                    title={this.state.title}
                    handleTitleChange={this.handleTitleChange}
                    onSubmit={this.saveTitleChange}
                />

                <ModifyDescriptionModal
                    showModal={this.state.showDescriptionModal}
                    hideModal={this.hideDescriptionModal}
                    description={this.state.description}
                    handleDescriptionChange={this.handleDescriptionChange}
                    onSubmit={this.saveDescription}
                />

                <SuccessModal
                    showModal={this.state.showSuccessModal}
                    hideModal={this.hideSuccessModal}
                    title={this.state.modalTitle}
                    successMessage={this.state.successMessage}
                />

                <ErrorModal
                    showModal={this.state.showErrorModal}
                    hideModal={this.hideErrorModal}
                    title={this.state.modalTitle}
                    errorMessage={this.state.errorMessage}
                />

            </div>
        );
    }
}