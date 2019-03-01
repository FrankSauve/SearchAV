import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import Loading from '../Loading';
import { ErrorModal } from '../Modals/ErrorModal';


interface State {
    file: any,
    loading: boolean,
    unauthorized: boolean,
    showErrorTranscribe: boolean,
    descriptionErrorTranscribe: string
}

export default class FileInput extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            file: null,
            loading: false,
            unauthorized: false,
            showErrorTranscribe: false,
            descriptionErrorTranscribe: ""
        }

    }
    
    public toggleLoad = () => {
        (this.state.loading) ? (this.setState({ loading: false })) : (this.setState({ loading: true }));
    };

    public showErrorModal = (description: string) => {
        this.setState({ showErrorTranscribe: true });
        this.setState({ descriptionErrorTranscribe: description });
    }

    public hideErrorModal = () => {
        this.setState({ showErrorTranscribe: false });
        this.setState({ descriptionErrorTranscribe: "" });
    }

    public getGoogleSample = (e:any) => {

        this.toggleLoad();

        this.setState({file: e.target.files[0]});

        const formData = new FormData();
        formData.append('audioFile', e.target.files[0]);
        formData.append('userEmail', auth.getEmail()!);

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'multipart/form-data'
            }
        };

        axios.post('/api/converter/convertandtranscribe', formData, config)
            .then(res => {
                this.toggleLoad();
            })
            .catch(err => {
                console.log(err)
                this.showErrorModal(err.response.data)
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public render() {
        
        return (
            <div className="column mg-top-30">
                <ErrorModal
                    showModal={this.state.showErrorTranscribe}
                    hideModal={this.hideErrorModal}
                    title={"Une erreur est survenu"}
                    errorMessage={this.state.descriptionErrorTranscribe}
                />
                <div className="file is-boxed has-name">
                    <label className="file-label">
                        <input className="file-input" type="file" name="File" onChange={this.getGoogleSample}/>
                        <span className="file-cta">
                            <span className="file-icon">
                                <i className="fas fa-upload"></i>
                            </span>
                            {this.state.loading ? <Loading/> : 
                                <span className="file-label">
                                    <br/>
                                    Ajouter un fichier...
                                    <br/>
                                    <br/>
                                </span>
                            }
                        </span>
                    </label>
                </div>
            </div>
        );
    }
}
