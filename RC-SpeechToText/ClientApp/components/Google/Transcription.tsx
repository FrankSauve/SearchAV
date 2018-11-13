import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

interface State { 
    audioFile: any,
    automatedTranscript: string,   
    loading: boolean
}

export default class Transcription extends React.Component<RouteComponentProps<{}>, State> {
    constructor(props: any){
      super(props);

      this.state = {
          audioFile: null,          
          automatedTranscript: '',          
          loading: false
      }
  }

    public getGoogleTranscription = () => {

        this.setState({ 'loading': true })

        const formData = new FormData();
        formData.append('audioFile', this.state.audioFile)

        const config = {
            headers: {
                'content-type': 'multipart/form-data'
            }
        }
        axios.post('/api/transcription/ManageTranscription', formData, config)
            .then(res => {
                this.setState({ 'loading': false });
                this.setState({ 'automatedTranscript': res.data.googleTranscriptionResult.alternatives[0].transcript });
            })
            .catch(err => console.log(err));
    }

    public onAddAudioFile = (e: any) => {
        this.setState({ audioFile: e.target.files[0] })
    }

  public render() {
      return (
      <div>
          <div className="container" >
            
            <h1 className="title mg-top-30">Google Speech To Text</h1>

            <div className="level">
                <div className="file has-name">
                    <label className="file-label">
                        <input className="file-input" type="file" name="resume" onChange={this.onAddAudioFile} />
                        <span className="file-cta">
                            <span className="file-icon">
                                <i className="fas fa-upload"></i>
                            </span>
                            <span className="file-label">
                                Fichier Audio…
                            </span>
                        </span>

                              <span className="file-name">
                                  {this.state.audioFile == null ? null : this.state.audioFile.name}
                        </span>
                    </label>
                </div>

            </div>

            <div className="level">
                {this.state.loading ? <a className="button is-danger is-loading"></a> : <a className="button is-danger" onClick={this.getGoogleTranscription}>Transcript</a>}
            </div>

            <h3 className="title is-3">{this.state.automatedTranscript == '' ? null : 'Automated transcript'}</h3>
            <p>{this.state.automatedTranscript}</p>
          
        </div>
      </div>
      );
  }
}