import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

interface State { 
    audioFile: any,
    srtFile: any,
    automatedTranscript :  string,
    manualTranscript: string,
    accuracy: number
    editTranscription: boolean,
    loading: boolean
 }

export class Home extends React.Component<RouteComponentProps<{}>, State> {

    constructor(props: any){
        super(props);

        this.state = {
            audioFile: null,
            srtFile: null,
            automatedTranscript: '',
            manualTranscript: '',
            editTranscription: false,
            accuracy: 0,
            loading: false
        }
    }

    public getGoogleSample = () => {
        this.setState({'loading': true})

        const formData = new FormData();
        formData.append('audioFile', this.state.audioFile)
        formData.append('srtFile', this.state.srtFile)

        const config = {
            headers: {
                'content-type': 'multipart/form-data'
            }
        }
        axios.post('/api/sampletest/googlespeechtotext', formData, config)
            .then(res => {
                this.setState({'loading': false});
                this.setState({ 'automatedTranscript': res.data.googleResponse.alternatives[0].transcript });
                this.setState({'editTranscription': true});
                this.setState({'manualTranscript': res.data.manualTranscript});
                this.setState({'accuracy': res.data.accuracy});
            })
            .catch(err => console.log(err));
    }

    public onAddAudioFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]})
    }

    public onAddSrtFile = (e: any) => {
        this.setState({srtFile: e.target.files[0]})
    }


    loadingCircle = <div className="preloader-wrapper active mg-top-30">
                    <div className="spinner-layer spinner-red-only">
                    <div className="circle-clipper left">
                        <div className="circle"></div>
                    </div><div className="gap-patch">
                        <div className="circle"></div>
                    </div><div className="circle-clipper right">
                        <div className="circle"></div>
                    </div>
                    </div>
                </div>

    editTranscriptionButton = <button className="waves-effect waves-light btn red">
                                Edit Transcription
                              </button>

    audioFileUpload = <div className="file-field input-field">
                        <div className="btn red">
                            <span>Audio File</span>
                            <input type="file" onChange={this.onAddAudioFile}/>
                        </div>
                        <div className="file-path-wrapper">
                            <input className="file-path validate" type="text"/>
                        </div>
                    </div>
    
    srtFileUpload = <div className="file-field input-field">
                        <div className="btn red">
                            <span>SRT File</span>
                            <input type="file" onChange={this.onAddSrtFile}/>
                        </div>
                        <div className="file-path-wrapper">
                            <input className="file-path validate" type="text"/>
                        </div>
                    </div>

    public render() {
        return (
        <div>
            <div className="container">
                <h1>SearchAV</h1>
                {this.audioFileUpload}
                {this.srtFileUpload}
                <a className="waves-effect waves-light btn red" onClick={this.getGoogleSample}>Google sample</a>
                <br/>
            
                {this.state.loading ? this.loadingCircle : null}
                
                <h3>{this.state.automatedTranscript == '' ? null : 'Automated transcript'}</h3>
                    <p>{this.state.automatedTranscript}</p>
                    {this.state.editTranscription ? this.editTranscriptionButton : null}

                <h3>{this.state.manualTranscript == '' ? null : 'Manual transcript'}</h3>
                <p>{this.state.manualTranscript}</p>

                <h3>{this.state.accuracy == 0 ? null : 'Accuracy'}</h3>
                <p>{this.state.accuracy == 0 ? null : this.state.accuracy}</p>
            </div>
        </div>
        );
    }
}
