import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';


interface State {
    file: any,
    loading: boolean,
    unauthorized: boolean
}

export default class FileInput extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            file: null,
            loading: false,
            unauthorized: false
        }

    }
    
    public toggleLoad = () => {
        (this.state.loading) ? (this.setState({ loading: false })) : (this.setState({ loading: true }));
    };

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

        axios.post('/api/transcription/convertandtranscribe', formData, config)
            .then(res => {
                this.toggleLoad();
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public render() {
        const progressBar = <img width="100" height="100" src="assets/loading.gif" alt="Loading..."/>
        
        return (
            <div className="column mg-top-30">
                <div className="file is-boxed has-name">
                    <label className="file-label">
                        <input className="file-input" type="file" name="File" onChange={this.getGoogleSample}/>
                        <span className="file-cta">
                            <span className="file-icon">
                                <i className="fas fa-upload"></i>
                            </span>
                            {this.state.loading ? progressBar : 
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
