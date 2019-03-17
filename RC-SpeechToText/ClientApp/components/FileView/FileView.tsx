import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { TranscriptionText } from './TranscriptionText';
import { TranscriptionHistorique } from './TranscriptionHistorique';
import { VideoPlayer } from './VideoPlayer';
import { TranscriptionSearch } from './TranscriptionSearch';
import { SaveTranscriptionButton } from './SaveTranscriptionButton';
import { FileInfo } from './FileInfo';
import Loading from '../Loading';
import { ModifyDescriptionModal } from '../Modals/ModifyDescriptionModal';
import { SuccessModal } from '../Modals/SuccessModal';
import { ErrorModal } from '../Modals/ErrorModal';

interface State {
    fileId: number,
    version: any,
    file: any,
    user: any,
    editedTranscript: string,
    unauthorized: boolean,
    fileTitle: string,
    description: string,
    showDescriptionModal: boolean,
    newDescription: string,
    showErrorModal: boolean,
    showSuccessModal: boolean,
    modalTitle: string,
    errorMessage: string,
    successMessage: string,
    showDropdown: boolean,
    loading: boolean,
    seekTime: string
}

export default class FileView extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            fileId: this.props.match.params.id,
            version: null,
            file: null,
            user: null,
            editedTranscript: "",
            unauthorized: false,
            fileTitle: "",
            description: "",
            showDescriptionModal: false,
            newDescription: "", 
            showErrorModal: false,
            showSuccessModal: false,
            modalTitle: "",
            errorMessage: "",
            successMessage: "",
            showDropdown: false,
            loading: false,
            seekTime: '0:00:00.00'
        }
    }

    // Called when the component is rendered
    public componentDidMount() {
        this.getVersion();
        this.getFile();
        this.getUser();
        this.setState({ description: this.state.description }); 
        document.addEventListener('mouseup', this.hideDropdown);
    }

    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.hideDropdown);
    }

    public getVersion = () => {
        this.setState({ loading: true });
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/version/GetActiveByFileId/' + this.state.fileId, config)
            .then(res => {
                this.setState({ version: res.data });
                this.setState({ editedTranscript: this.state.version.transcription }); //Will avoid to have empty transcript except if user erase everything
                this.setState({ loading: false });
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getFile = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/details/' + this.state.fileId, config)
            .then(res => {
                this.setState({ file: res.data }); 
                this.setState({ description: this.state.file.description }); 
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }


    public getUser = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/user/getUserByEmail/' + auth.getEmail(), config)
            .then(res => {
                this.setState({ user: res.data });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
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

    public handleTranscriptChange = (text: string) => {
        this.setState({ editedTranscript: text });
    }

    public updateVersion = (newVersion: any) => {
        this.setState({ version: newVersion });
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

    public handleDescriptionChange = (event: any) => {
        this.setState({ newDescription: event.target.value });
    }

    public hideSuccessModal = () => {
        this.setState({ showSuccessModal: false });
    }

    public hideErrorModal = () => {
        this.setState({ showErrorModal: false });
    }

    public showDropdown = () => {
        this.setState({ showDropdown: true });
    }

    public hideDropdown = () => {
        this.setState({ showDropdown: false });
    }


    public handleSeekTime = (time: string) => {
        this.setState({ seekTime: time });
    };

    render() {
        return (
            <div className="container">

                <div className="columns">
                    <div className="column is-one-third mg-top-30 has-background-white-smoke">
                        {/* Using title for now, this will have to be change to path eventually */}
                        {this.state.file ? <VideoPlayer path={this.state.file.title} seekTime={this.state.seekTime} /> : null}

                        {this.state.file ? <b className="has-text-link">Titre: </b> : null}
                        {this.state.file ? (this.state.file.title ? <div><div className="card">
                            <div className="card-content">
                                {this.state.file.title}
                            </div> </div></div> : <div className="card">
                                <div className="card-content"> This file has no title </div></div>) : null}
                        
                        <br />

                        {this.state.file ? <b className="has-text-link">Description: <a onClick={this.showDescriptionModal}><i className="fas fa-edit"></i></a></b> : null}
                        {this.state.file ? (this.state.file.description ? <div>
                            <div className="card">
                                <div className="card-content">
                                {this.state.description}
                                </div>

                            </div></div> : <div className="card">
                                <div className="card-content"> This file has no description </div></div>) : null}
                        
                        <br />

                        <p>{this.state.file ? (this.state.file ? <div><div className="card">
                            <div className="card-content">
                                {this.state.version ?
                                    <FileInfo
                                        thumbnail={this.state.file.thumbnailPath}
                                        userId={this.state.file.userId}
                                        dateModified={this.state.version.dateModified} /> : null}
                            </div> </div></div> : <div className="card">
                                <div className="card-content"> This file has no extra Info </div></div>) : null}</p>
                    </div>

                    <div className="column is-half mg-top-30">
                        {this.state.version ? <TranscriptionSearch versionId={this.state.version.id} /> : null}
                        {this.state.loading ?
                            <Loading />
                            : this.state.version && this.state.file && this.state.user ?
                                <div>
                                    <TranscriptionText
                                        text={this.state.version.transcription}
                                        version={this.state.version}
                                        handleChange={this.handleTranscriptChange}
                                        handleSeekTime={this.handleSeekTime} />
                                    <SaveTranscriptionButton
                                        version={this.state.version}
                                        updateVersion={this.updateVersion}
                                        editedTranscription={this.state.editedTranscript}
                                        reviewerId={this.state.file.reviewerId}
                                        userId={this.state.user.id}
                                    />
                                </div>
                                : null}

                    </div>

                    <div className="column is-one-fifth historique_padding">
                        <TranscriptionHistorique
                            fileId={this.state.fileId}
                        />
                    </div>

                    <div>
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

                </div>
            </div>

        )
    }
}
