import * as React from 'react';
import axios from "axios";
import auth from '../../Utils/auth';
import { Redirect } from 'react-router-dom';


interface State{
    fullGoogleResponse:any,
    unauthorized: boolean
}

export class TranscriptionButton extends React.Component<any, State> {
    constructor(props: any) {
        super(props);
        
        this.state ={
            fullGoogleResponse:null,
            unauthorized: false
        }

    }

    public getGoogleSample = () => {
        
        this.props.toggleLoad();
        
        const formData = new FormData();
        formData.append('audioFile', this.props.audioFile);

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'multipart/form-data'
            }
        };
        
        let fullGoogleResponse=null;
        
        axios.post('/api/transcription/convertandtranscribe', formData, config)
            .then(res => {
                fullGoogleResponse = res.data.googleResponse.alternatives[0];

                this.props.updateTranscript(fullGoogleResponse);
                this.props.toggleLoad();
            })
            .catch(err => {
                if(err.response.status == 401) {
                    this.setState({'unauthorized': true});
                }
            });
    };
    
    static showError(){
        alert('File Not Found');
    }

    public render() {
        
        return (
            <div>
                
                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}

                <div className="level">
                    {(this.props.loading) ? <a className="button is-danger is-loading">Go</a> : 
                        <a className="button is-danger" onClick={(this.props.audioFile != null) ? this.getGoogleSample : TranscriptionButton.showError}>Go</a>
                    }
                </div>
            </div>
        );
    }
}
