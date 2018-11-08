import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

interface State { 
    audioFile: any,
    srtFile: any,
    automatedTranscript: string,
    manualTranscript: string,
    accuracy: number
    editTranscription: boolean,
    textarea: boolean,
    submitEdit: boolean,
    loading: boolean
}

export default class Google extends React.Component<RouteComponentProps<{}>, State> {
    constructor(props: any){
        super(props);
        //this.handleChange = this.handleChange.bind(this);

      this.state = {
          audioFile: null,
          srtFile: null,
          automatedTranscript: '',
          manualTranscript: '',
          editTranscription: false,
          textarea: false,
          submitEdit: false,
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
              this.setState({ 'loading': false });
              this.setState({ 'automatedTranscript': res.data.googleResponse.alternatives[0].transcript });
              this.setState({ 'editTranscription': true });
              this.setState({ 'manualTranscript': res.data.manualTranscript });
              this.setState({ 'accuracy': res.data.accuracy });
          })
          .catch(err => console.log(err));
  }

  public onAddAudioFile = (e: any) => {
      this.setState({audioFile: e.target.files[0]})
  }

  public onAddSrtFile = (e: any) => {
      this.setState({srtFile: e.target.files[0]})
    }

   /*handleChange = (e: any) => {
        this.setState({ automatedTranscript: e.target.value })
    }*/
    
   public editTranscription = () => {
       this.setState({ 'submitEdit': true })
       this.setState({ 'editTranscription': false })
       this.setState({ 'textarea': true })
    }

    public handleSubmit = (e: any) => {
        this.setState({ 'automatedTranscript': e.target.value })
        this.setState({ 'submitEdit': false })
        this.setState({ 'editTranscription': true })
        this.setState({ 'textarea': false })
    }

    editTranscriptionButton = <button className="waves-effect waves-light btn red" onClick={this.editTranscription}>
                                    Edit Transcription
                              </button>

    automatedTranscriptionTextarea = <textarea
                                        //onChange={this.handleChange}
                                        rows={10} //Would be nice to adapt this to the number of lines in the future
                                        defaultValue=''
                                      />

    submitEditButton = <button className="waves-effect waves-light btn red" onClick={this.handleSubmit}>
                            Submit
                       </button>

  public render() {
      return (
      <div>
          <div className="container" >
            
            <h1 className="title mg-top-30">Google Speech To Text</h1>

            <div className="level mg-top-30">
              <div className="file has-name">
                <label className="file-label">
                    <input className="file-input" type="file" name="resume" onChange={this.onAddAudioFile}/>
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
              <div className="file has-name">
                <label className="file-label">
                    <input className="file-input" type="file" name="resume" onChange={this.onAddSrtFile}/>
                    <span className="file-cta">
                    <span className="file-icon">
                        <i className="fas fa-upload"></i>
                    </span>
                    <span className="file-label">
                        Ficher SRT…
                    </span>
                    </span>
                    <span className="file-name">
                    {this.state.srtFile == null ? null : this.state.srtFile.name}
                    </span>
                </label>
              </div>
            </div>

            <div className="level">
                {this.state.loading ? <a className="button is-danger is-loading"></a> : <a className="button is-danger" onClick={this.getGoogleSample}>Go</a>}
            </div>

            
            <h3 className="title is-3">{this.state.automatedTranscript == '' ? null : 'Automated transcript'}</h3>
                  <p>{this.state.automatedTranscript}</p>
                  {this.state.editTranscription ? this.editTranscriptionButton : null}
                  {this.state.textarea ? this.automatedTranscriptionTextarea : null}
                  {this.state.submitEdit ? this.submitEditButton : null}
          
            <h3 className="title is-3">{this.state.manualTranscript == '' ? null : 'Manual transcript'}</h3>
            <p>{this.state.manualTranscript}</p>
          
            <h3 className="title is-3">{this.state.accuracy == 0 ? null : 'Accuracy'}</h3>
            <p>{this.state.accuracy == 0 ? null : this.state.accuracy}</p>
          
        </div>
      </div>
      );
  }
}
