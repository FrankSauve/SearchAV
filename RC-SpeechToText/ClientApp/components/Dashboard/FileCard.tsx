import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Link } from 'react-router-dom';

interface State {
    title: string,
    description: string,
    modifiedTitle: string,
    newDescription: string,
    showDropdown: boolean,
    showTitleModal: boolean,
    showDescriptionModal: boolean,
    unauthorized: boolean,
}

export class FileCard extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            title: this.props.title,
            description: this.props.description,
            modifiedTitle: "",
            newDescription:"",
            showDropdown: false,
            showTitleModal: false,
            showDescriptionModal: false,
            unauthorized: false
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

    //public saveDescription = () => {
    //    var oldDescription = this.state.file.description
    //    var newDescription = this.state.description

    //    const formData = new FormData();
    //    formData.append("fileId", this.state.fileId + '')
    //    formData.append("newDescription", newDescription + '')

    //    if (oldDescription != newDescription) {
    //        const config = {
    //            headers: {
    //                'Authorization': 'Bearer ' + auth.getAuthToken(),
    //                'content-type': 'application/json'
    //            }
    //        }

    //        axios.put('/api/file/saveDescription', formData, config)
    //            .then(res => {
    //                this.setState({ description: this.state.newDescription });
    //                this.hideDescriptionModal();
    //            })
    //            .catch(err => {
    //                if (err.response.status == 401) {
    //                    this.setState({ 'unauthorized': true });
    //                }
    //            });
    //    }
    //}

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

    public render() {
        return (
            <div className="column is-3">

                <div className={`modal ${this.state.showTitleModal ? "is-active" : null}`} >
                    <div className="modal-background"></div>
                    <div className="modal-card">
                        <header className="modal-card-head">
                            <p className="modal-card-title">Modifier le titre</p>
                            <button className="delete" aria-label="close" onClick={this.hideTitleModal}></button>
                        </header>
                        <section className="modal-card-body">
                            <div className="field">
                                <div className="control">
                                    <input className="input is-primary" type="text" placeholder={this.state.title ?
                                        (this.state.title.lastIndexOf('.') != -1 ? this.state.title.substring(0, this.state.title.lastIndexOf('.'))
                                            : this.state.title) : "Enter title"} defaultValue={this.state.title} onChange={this.handleTitleChange} />
                                </div>
                            </div>
                        </section>
                        <footer className="modal-card-foot">
                            <button className="button is-success" onClick={this.saveTitleChange}>Enregistrer</button>
                            <button className="button" onClick={this.hideTitleModal}>Annuler</button>
                        </footer>
                    </div>
                </div>

                <div className={`modal ${this.state.showDescriptionModal ? "is-active" : null}`} >
                    <div className="modal-background"></div>
                    <div className="modal-card">
                        <header className="modal-card-head">
                            <p className="modal-card-title">{this.state.description && this.state.description != "" ? "Modifier une description" : "Ajouter une description" }</p>
                            <button className="delete" aria-label="close" onClick={this.hideDescriptionModal}></button>
                        </header>
                        <section className="modal-card-body">
                            <div className="field">
                                <div className="control">
                                    <textarea className="textarea is-primary" type="text" placeholder={this.state.description && this.state.description != "" ?
                                        this.state.description : "Enter description"} defaultValue={this.state.description ? this.state.description : ""}
                                        onChange={this.handleDescriptionChange} />
                                </div>
                            </div>
                        </section>
                        <footer className="modal-card-foot">
                            <button className="button is-success">Enregistrer</button>
                            <button className="button" onClick={this.hideDescriptionModal}>Annuler</button>
                        </footer>
                    </div>
                </div>

                <div className="card mg-top-30 fileCard">
                    <span className="tag is-danger is-rounded">{this.props.flag}</span>
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
            </div>
        );
    }
}