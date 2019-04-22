import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import Loading from '../Loading';
import { ErrorModal } from '../Modals/ErrorModal';
import { SuccessModal } from '../Modals/SuccessModal';
import { AddTitleDescriptionModal } from '../Modals/AddTitleDescriptionModal';
import { AddMultipleFilesModal } from '../Modals/AddMultipleFilesModal';


interface State {
    file: any,
    loading: boolean,
    unauthorized: boolean,
    title: string,
    description: string,
    showAddTitleDescriptionModal: boolean,
    showSuccessTranscribe: boolean,
    showErrorModal: boolean,
    modalTitle: string,
    errorMessage: string,
    showAddMultipleFilesModal: boolean,
    count: number
}

export default class FileInput extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            file: null,
            loading: false,
            unauthorized: false,
            title: "",
            description: "",
            showAddTitleDescriptionModal: false, 
            showSuccessTranscribe: false,
            showErrorModal: false,
            modalTitle: "",
            errorMessage: "",
            showAddMultipleFilesModal: false,
            count: 0
        }

    }
    
    public toggleLoad = () => {
        (this.state.loading) ? (this.setState({ loading: false })) : (this.setState({ loading: true }));
    };

    public showAddTitleDescriptionModal = (e: any) => {
        this.setState({ file: e.target.files });
        this.setState({title: e.target.files[0].name.split(".")[0]}) // Name of the file without the extension
        this.setState({ count: e.target.files.length })
        if (e.target.files.length > 1) {
            this.setState({ showAddMultipleFilesModal: true });
        }
        else {
            this.setState({ showAddTitleDescriptionModal: true });
        }
    } 

    public hideAddTitleDescriptionModal = () => {
        this.setState({ showAddTitleDescriptionModal: false }); 
    }

    public hideAddTitleDescriptionModalThenRefresh = () => {
        this.setState({ showAddTitleDescriptionModal: false });
        window.location.reload();
    }

    public handleTitleChange = (event: any) => {
        this.setState({ title: event.target.value });
    }

    public handleDescriptionChange = (event: any) => {
        this.setState({ description: event.target.value });
    }

    public showSuccessModal = () => {
        this.setState({ showSuccessTranscribe: true });
    }

    public hideSuccessModal = () => {
        this.setState({ showSuccessTranscribe: false });
    }

    public showErrorModal = (title: string, description: string) => {
        this.setState({ errorMessage: description });
        this.setState({ modalTitle: title });
        this.setState({ showErrorModal: true });
    }

    public hideErrorModal = () => {
        this.setState({ showErrorModal: false });
        this.setState({ errorMessage: "" });
    }

    public hideAddMultipleFilesModal = () => {
        this.setState({ showAddMultipleFilesModal: false });
        window.location.reload();
    }


    public hideAddMultipleFileModal = () => {
        this.setState({ showAddMultipleFilesModal: false });
    }

    public getGoogleSample = () => { 
        this.hideAddMultipleFileModal(); 
        this.toggleLoad();
        const formData = new FormData();
        
        for (var i = 0; i < this.state.file.length; i++)
        {
            formData.append('files', this.state.file[i]);
            formData.append('userEmail', auth.getEmail()!);
            formData.append('description', this.state.description);
            formData.append('title', this.state.file[i].name.split(".")[0]); 
        }
      
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'multipart/form-data'
            }
        };

        axios.post('/api/converter/convertandtranscribe', formData, config)
            .then(res => {
                this.toggleLoad();
                //Updating files (maybe find a better to do it rather than load all entities every single time a file is uploaded)
                this.props.getAllFiles(); 
                this.showSuccessModal();
            })
            .catch(err => {
                this.toggleLoad();
                console.log(err.response.data);
                this.showErrorModal("Échec de l'importation!", err.response.data.message)
                this.props.getAllFiles();
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public verifyIfTitleExists = () => {
        var title = this.state.title;

        if (title != "")
        {
            const formData = new FormData();
            formData.append("title", title)

            const config = {
                headers: {
                    'Authorization': 'Bearer ' + auth.getAuthToken(),
                    'content-type': 'application/json'
                }
            }
            axios.post('/api/file/VerifyIfTitleExists/' + title.split(".")[0], formData, config)
                .then(res => {
                    this.hideAddTitleDescriptionModal();
                    this.getGoogleSample();
                })
                .catch(err => {
                    console.log(err);
                    if (err.response.data) {
                        this.showErrorModal("Modifier le titre", err.response.data.message);
                    }
                });
        }
        else {
            this.showErrorModal("Modifier le titre", "Title is null");
        }
    }


    onDrag = (e: any) => {
        e.preventDefault();
    }

    onDragOver = (e: any) => {
        e.preventDefault();
    }

    onDrop = (e: any) => {
        e.preventDefault();
        this.setState({ file: e.dataTransfer.files})
        this.setState({ title: e.dataTransfer.files[0].name.split(".")[0] }) // Name of the file without the extension
        this.setState({ count: e.dataTransfer.files.length })
        if (e.dataTransfer.files.length > 1) {
            this.setState({ showAddMultipleFilesModal: true });
        }
        else
        {
            this.setState({ showAddTitleDescriptionModal: true });
        }
    }

    public render() {
        return (
            <div className="column mg-top-30 no-padding">
                <AddTitleDescriptionModal
                showModal={this.state.showAddTitleDescriptionModal}
                hideModal={this.hideAddTitleDescriptionModalThenRefresh}
                handleDescriptionChange={this.handleDescriptionChange}
                handleTitleChange={this.handleTitleChange}
                title={this.state.title}
                onSubmit={this.verifyIfTitleExists}
                />

                <AddMultipleFilesModal
                    showModal={this.state.showAddMultipleFilesModal}
                    hideModal={this.hideAddMultipleFilesModal}
                    count={this.state.count}
                    onSubmit={this.getGoogleSample}
                />

                <ErrorModal
                    showModal={this.state.showErrorModal}
                    hideModal={this.hideErrorModal}
                    title={"Échec de l'importation!"}
                    errorMessage={this.state.errorMessage}
                />

                <SuccessModal
                    showModal={this.state.showSuccessTranscribe}
                    hideModal={this.hideSuccessModal}
                    title={"Importation Réussie!"}
                    successMessage="La transcription de votre fichier a été effectué avec succès. Vous recevrez un courriel dans quelques instants."
                />

                <div className="file is-boxed has-name"
                    onDrop={e => this.onDrop(e)}
                    onDrag={(e => this.onDrag(e))}
                    onDragOver={(e => this.onDragOver(e))}
                >
                    <label className="file-label">
                        <input className="file-input" type="file" name="File" multiple onChange={this.showAddTitleDescriptionModal} />
                        <span className="file-cta no-border">
                            {this.state.loading ? <Loading /> :
                                <span className="file-icon">
                                    <i className="fas fa-cloud-upload-alt"></i>
                                </span>
                            }
                            {this.state.loading ? null : 
                                <span className="file-label">
                                    <div className="file-input-text">Glisser les fichier ici</div>
                                    <div className="file-input-text">ou</div>
                                    <div className="button is-link button-parcourir">Parcourir</div>
                                </span>
                            }
                        </span>
                    </label>
                </div>
            </div>
        );
    }
}
