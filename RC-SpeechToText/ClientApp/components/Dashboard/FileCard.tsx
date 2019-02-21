import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Link } from 'react-router-dom';
import { ModifyTitleModal } from '../Modals/ModifyTitleModal';
import { ModifyDescriptionModal } from '../Modals/ModifyDescriptionModal';
import { SuccessModal } from '../Modals/SuccessModal';
import { ErrorModal } from '../Modals/ErrorModal';

interface State {
    title: string,
    description: string,
    modifiedTitle: string,
    newDescription: string,
    flag: string,
    notified: number,
    showDropdown: boolean,
    showTitleModal: boolean,
    showDescriptionModal: boolean,
    showErrorDescriptionModal: boolean,
    showSuccessDescriptionModal: boolean,
    unauthorized: boolean,
}

export class FileCard extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            title: this.props.title,
            description: this.props.description,
            modifiedTitle: "",
            newDescription: "",
            flag: this.props.flag,
            notified: this.props.number, 
            showDropdown: false,
            showTitleModal: false,
            showDescriptionModal: false,
            showErrorDescriptionModal: false,
            showSuccessDescriptionModal: false,
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
        axios.delete('/api/word/DeleteByFileId/' + this.props.fileId, config)
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

        axios.delete('/api/version/DeleteFileVersions/' + this.props.fileId, config)
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
        axios.delete('/api/file/Delete/' + this.props.fileId, config)
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

        var newTitle = this.state.modifiedTitle

        const formData = new FormData();
        formData.append("newTitle", newTitle)

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }

        axios.put('/api/file/ModifyTitle/' + this.props.fileId, formData, config)
            .then(res => {
                this.setState({ title: this.state.modifiedTitle });
                this.hideTitleModal();
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });

    }

    public saveDescription = () => {

        var oldDescription = this.state.description
        var newDescription = this.state.newDescription

        const formData = new FormData();
        formData.append("newDescription", newDescription)

        if (oldDescription != newDescription && newDescription != "") {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            }

            axios.put('/api/file/saveDescription/' + this.props.fileId, formData, config)
                .then(res => {
                    this.setState({ description: this.state.newDescription });
                    this.hideDescriptionModal();
                    this.showSuccessDescriptionModal();
                })
                .catch(err => {
                    if (err.response.status == 401) {
                        this.setState({ 'unauthorized': true });
                    }
                });
        }
        else {
            this.showErrorDescriptionModal();
        }
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

    public showSuccessDescriptionModal = () => {
        this.setState({ showSuccessDescriptionModal: true });
    }

    public hideSuccessDescriptionModal = () => {
        this.setState({ showSuccessDescriptionModal: false });
    }

    public showErrorDescriptionModal = () => {
        this.setState({ showErrorDescriptionModal: true });
    }

    public hideErrorDescriptionModal = () => {
        this.setState({ showErrorDescriptionModal: false });
    }

    public render() {
        return (
            <div className="column is-3">
                <div className="card fileCard">
                    <span className={`tag is-rounded ${this.state.flag.indexOf("A") == 0 ? "is-danger" : this.state.flag.indexOf("R") == 0 ? "is-success" : "is-info"}`}>{this.state.flag}</span> 
                    <header className="card-header">
                        <p className="card-header-title fileTitle">
                            {this.state.title ? (this.state.title.lastIndexOf('.') != -1 ? this.state.title.substring(0, this.state.title.lastIndexOf('.')) : this.state.title) : null}</p>
                        <div className={`dropdown ${this.state.showDropdown ? "is-active" : null}`} >
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
                    </header>
                    <div className="card-image">
                        <div className="hovereffect">
                            <figure className="image is-4by3">
                                <img src={this.props.image} alt="Placeholder image" />
                                <div className="overlay">
                                    <Link className="info" to={`/FileView/${this.props.fileId}`}>View/Edit</Link>
                                </div>
                            </figure>
                        </div>
                    </div>
                    <div className="card-content">
                        <div className="content fileContent">
                            <p className="transcription">{this.props.transcription}</p>
                            <p><b>{this.props.username}</b></p>
                            <time dateTime={this.props.date}>{this.props.date}</time>
                            {/* <p><b>Description:</b> {this.state.description}</p> */}
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
                    showModal={this.state.showSuccessDescriptionModal}
                    hideModal={this.hideSuccessDescriptionModal}
                    title={this.state.description && this.state.description != "" ? "Modifier la description" : "Ajouter une description"}
                    successMessage="Enregistrement de la description confirm�! Les changements effectu�s ont �t� enregistr� avec succ�s."
                />

                <ErrorModal
                    showModal={this.state.showErrorDescriptionModal}
                    hideModal={this.hideErrorDescriptionModal}
                    title={this.state.description && this.state.description != "" ? "Modifier la description" : "Ajouter une description"}
                    errorMessage="Enregistrement de la description annul�! Vous n'avez effectu� aucun changements ou vous avez apport� les m�mes modifications."
                />

            </div>
        );
    }
}