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
import { SuccessModal } from '../Modals/SuccessModal';
import { ErrorModal } from '../Modals/ErrorModal';
import { DescriptionText } from './DescriptionText';
import { ConfirmationModal } from '../Modals/ConfirmationModal';
import { LoadingModal } from '../LoadingModal';

interface State {
    fileId: AAGUID,
    version: any,
    file: any,
    user: any,
    editedTranscript: string,
    unauthorized: boolean,
    fileTitle: string,
    description: string,
    showConfirmDescriptionModal: boolean,
    newDescription: string,
    showErrorModal: boolean,
    showSuccessModal: boolean,
    modalTitle: string,
    errorMessage: string,
    successMessage: string,
    showDropdown: boolean,
    loading: boolean,
    loadingModal: boolean,
    seekTime: string,
    timestampInfo: string,
    selection: string,
    versions: any[],
    usernames: string[], 
    textSearch: boolean,
    versionToDiff: any;
    path: string
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
            showConfirmDescriptionModal: false,
            newDescription: "", 
            showErrorModal: false,
            showSuccessModal: false,
            modalTitle: "",
            errorMessage: "",
            successMessage: "",
            showDropdown: false,
            loading: false,
            loadingModal: false,
            seekTime: '0:00:00.00',
            timestampInfo: '0:00:00.00-0:00:00.00',
            selection: '',
            versions: [],
            usernames: [], 
            textSearch: false,
            versionToDiff: null,
            path: ""
        }
    }

    // Called when the component is rendered
    public componentDidMount() {
        this.getVersion();
        this.getFile();
        this.getUser();
        this.getAllVersions();
        this.setState({ description: this.state.description }); 
        document.addEventListener('mouseup', this.hideDropdown);
    }

    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.hideDropdown);
    }

    public getAllVersions = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/version/GetAllVersionsWithUserName/' + this.state.fileId, config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'versions': res.data.versions });
                this.setState({ 'usernames': res.data.usernames });
                
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };


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
    };

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
                this.setState({ path: this.state.file.filePath });
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };


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
    };
    public updateDiff = (e: any) => {
        this.setState({'versionToDiff' : e});
    }

    public saveDescription = () => {
        this.hideConfirmDescriptionModal();
        this.setState({ loadingModal: true });

        var oldDescription = this.state.description;
        var newDescription = this.state.newDescription;

        var modalTitle = (this.state.description && this.state.description != "" ? "Modifier la description" : "Ajouter une description");

        const formData = new FormData();
        formData.append("newDescription", newDescription)

        if (oldDescription != newDescription && newDescription != "") {
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            };

            axios.put('/api/file/saveDescription/' + this.state.fileId, formData, config)
                .then(res => {
                    this.setState({ description: this.state.newDescription });
                    this.setState({ loadingModal: false });
                    this.showSuccessModal(modalTitle, "Enregistrement de la description confirmé! Les changements effectués ont été enregistré avec succès.");
                })
                .catch(err => {
                    if (err.response.status == 401) {
                        this.setState({ loadingModal: false });
                        this.showErrorModal(modalTitle, "Veuillez vous connecter avant de modifier la description.");
                        this.setState({ 'unauthorized': true });
                    }
                });
        }
        else {
            this.setState({ loadingModal: false });
            this.showErrorModal(modalTitle, "Enregistrement de la description annulé! Vous n'avez effectué aucun changements ou vous avez apporté les mêmes modifications.");
        }
    };

    // Searches the transcript for the selection, and updates the seekTime var with its timestamp
    public searchTranscript = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        };

        // Search the entire selection for now
        // This will probably have to change by having words with timestamps in the frontend
        // But its an ok temporary solution
        axios.get('/api/Transcription/SearchTranscript/' + this.state.version.id + '/' + this.state.selection , config)
            .then(res => {
                this.handleSeekTime(res.data);
                this.setState({timestampInfo: res.data});
                this.handleTextSearch(true);
            })
            .catch(err => {
                console.log(err);
            });
    };
    
    public handleSelectionChange = (sel : string) =>{
        if(sel != null && sel != '' && sel != ' ') {
            this.setState({selection: sel}, () =>{
                this.searchTranscript();
            });
        }
    };
    
    public handleTextSearch = (b: boolean) =>{
        this.setState({textSearch: b});
    };

    public handleSeekTime = (time: string) => {
        this.setState({ seekTime: time });
    };

    public handleTranscriptChange = (text: string) => {
        this.setState({ editedTranscript: text });
    };

    public updateVersion = (newVersion: any) => {
        this.setState({ version: newVersion });
    };

    public showConfirmDescriptionModal = () => {
        this.setState({ showConfirmDescriptionModal: true });
    };

    public hideConfirmDescriptionModal = () => {
        this.setState({ showConfirmDescriptionModal: false });
    };

    public showSuccessModal = (title: string, description: string) => {
        this.setState({ successMessage: description });
        this.setState({ modalTitle: title });
        this.setState({ showSuccessModal: true });
    };

    public showErrorModal = (title: string, description: string) => {
        this.setState({ errorMessage: description });
        this.setState({ modalTitle: title });
        this.setState({ showErrorModal: true });
    };

    public handleDescriptionChange = (text: string) => {
        this.setState({ newDescription: text })
    };

    public hideSuccessModal = () => {
        this.setState({ showSuccessModal: false });
    };

    public hideErrorModal = () => {
        this.setState({ showErrorModal: false });
    };

    public showDropdown = () => {
        this.setState({ showDropdown: true });
    };

    public hideDropdown = () => {
        this.setState({ showDropdown: false });
    };

    removeExtension(title: string) {
        var titleNoExtension = title.lastIndexOf('.') != -1 ? title.substring(0, title.lastIndexOf('.')) : title;
        return titleNoExtension;
    };

    render() {
        var modalTitle = (this.state.description && this.state.description != "" ? "Modifier la description" : "Ajouter une description");
        return (
            <div className="container">

                <div className="columns">
                    <div className="column is-one-third file-view-info-section">
                        {/* Using title for now, this will have to be change to path eventually */}
                        {this.state.file ? <VideoPlayer path={this.state.file.filePath} seekTime={this.state.seekTime} controls={true}/> : null}

                        {this.state.file ? <b className="file-view-header">Titre: </b> : null}
                        {this.state.file ? (this.state.file.title ? <div>
                            <div className="file-view-title">
                                {this.removeExtension(this.state.file.title)}
                            </div> </div> : <div className="file-view-title"> Ce fichier n'a pas de titre </div>) : null}
                        
                        <br />

                        {this.state.file ? <b className="file-view-header">Description: <a onClick={this.showConfirmDescriptionModal}><i className="fas fa-edit mg-left-5"></i></a></b> : null}
                        {this.state.file ?
                            <div>
                                <DescriptionText
                                    description={this.state.file.description ? this.state.file.description : "Ce fichier n'a aucune description."}
                                    handleDescriptionChange={this.handleDescriptionChange}
                                />
                        </div> : null}
                        
                        <br />

                        <p>{this.state.file ? (this.state.version ?
                            <FileInfo
                                thumbnail={this.state.file.thumbnailPath}
                                userId={this.state.file.userId}
                                dateModified={this.state.version.dateModified}
                                file={this.state.file}
                            /> : <p>This file has no extra Info </p>)
                            : null}</p>
                    </div>

                    <div className="column is-half mg-top-30 editing-section">
                        {this.state.version ? 
                            <TranscriptionSearch 
                                versionId={this.state.version.id} 
                                handleSeekTime={this.handleSeekTime} 
                                selection={this.state.selection}
                                handleSelectionChange={this.handleSelectionChange} /> : null}
                        {this.state.loading ?
                            <Loading />
                            : this.state.version && this.state.file && this.state.user ?
                                <div>
                                    <TranscriptionText
                                        text={this.state.version.transcription}
                                        version={this.state.version}
                                        versionToDiff={this.state.versionToDiff}
                                        handleTranscriptChange={this.handleTranscriptChange}
                                        handleSeekTime={this.handleSeekTime}
                                        selection={this.state.selection}
                                        handleSelectionChange={this.handleSelectionChange}
                                        textSearch={this.state.textSearch}
                                        handleTextSearch={this.handleTextSearch}
                                        timestampInfo={this.state.timestampInfo}/>
                                    <SaveTranscriptionButton
                                        version={this.state.version}
                                        updateVersion={this.updateVersion}
                                        editedTranscription={this.state.editedTranscript}
                                        reviewerId={this.state.file.reviewerId}
                                        userId={this.state.user.id}
                                        getAllVersions={this.getAllVersions}
                                    />
                                </div>
                                : null}

                    </div>

                    <div className="column is-one-fifth historique_padding">
                        <TranscriptionHistorique
                            fileId={this.state.fileId}
                            versions={this.state.versions}
                            usernames={this.state.usernames}
                            versionToDiff={this.state.versionToDiff}
                            updateDiff={this.updateDiff}
                        />
                    </div>

                    <div>

                        <ConfirmationModal
                            showModal={this.state.showConfirmDescriptionModal}
                            title={modalTitle}
                            hideModal={this.hideConfirmDescriptionModal}
                            confirmMessage={this.state.file ? "Etes-vous sur(e) de modifier la description du fichier '" + this.state.file.title + "' ?" : "Etes-vous sur(e) de modifier la description de ce fichier ?"}
                            confirmButton={"Confirmer"}
                            onConfirm={this.saveDescription}
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

                        <LoadingModal
                            showModal={this.state.loadingModal}
                        />

                    </div>

                </div>
            </div>

        )
    }
}
