import * as React from 'react';
import axios from "axios";


interface State{
    fullGoogleResponse:any
}

export class TranscriptionButton extends React.Component<any> {
    constructor(props: any) {
        super(props);
        
        this.state ={
            fullGoogleResponse:null
        }

    }

    public getGoogleSample = () => {
        
        this.props.toggleLoad();
        
        const formData = new FormData();
        formData.append('audioFile', this.props.audioFile);

        const config = {
            headers: {
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
            .catch(err => console.log(err));
    };
    
    static showError(){
        alert('File Not Found');
    }

    public render() {
        
        return (
            <div>
                <div className="level">
                    {(this.props.loading) ? <a className="button is-danger is-loading">Go</a> : 
                        <a className="button is-danger" onClick={(this.props.audioFile != null) ? this.getGoogleSample : TranscriptionButton.showError}>Go</a>
                    }
                </div>
            </div>
        );
    }
}
