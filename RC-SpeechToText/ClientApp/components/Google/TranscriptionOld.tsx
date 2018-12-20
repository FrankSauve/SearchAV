import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

interface State {
    audioFile: any,
    automatedTranscript: string,
    showAutomatedTranscript: boolean,
    fullGoogleResponse: any,
    editTranscription: boolean,
    saveTranscription: boolean,
    textarea: boolean,
    submitEdit: boolean,
    editSuccess: boolean,
    loading: boolean
    searchTerms: string,
    timestamps: string,
}

export default class TranscriptionOld extends React.Component<RouteComponentProps<{}>, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null,
            automatedTranscript: '',
            showAutomatedTranscript: false,
            fullGoogleResponse: null,
            editTranscription: false,
            saveTranscription: false,
            textarea: false,
            submitEdit: false,
            editSuccess: false,
            loading: false,
            searchTerms: '',
            timestamps: '',
        }
    }

    public getGoogleSample = () => {
        this.setState({ 'loading': true })

        const formData = new FormData();
        formData.append('audioFile', this.state.audioFile)

        const config = {
            headers: {
                'content-type': 'multipart/form-data'
            }
        }
        axios.post('/api/transcription/convertandtranscribe', formData, config)
            .then(res => {
                this.setState({ 'loading': false });
                this.setState({ 'automatedTranscript': res.data.googleResponse.alternatives[0].transcript });
                this.setState({ 'fullGoogleResponse': res.data.googleResponse.alternatives[0] });
                this.setState({ 'showAutomatedTranscript': true })
                this.setState({ 'editTranscription': true });
                this.setState({ 'editSuccess': false });
            })
            .catch(err => console.log(err));

        console.log(this.state.fullGoogleResponse);

    }

    public searchTranscript = () => {

        const formData = new FormData();
        formData.append('searchTerms', this.state.searchTerms)
        formData.append('jsonResponse', JSON.stringify(this.state.fullGoogleResponse))

        const config = {
            headers: {
                'content-type': 'application/json'
            },
        }

        axios.post('/api/TranscriptSearch/SearchTranscript', formData, config)
            .then(res => {
                this.setState({ 'timestamps': res.data });
            })
            .catch(err => console.log(err));
    }

    // public saveTranscription = () => {
    //     this.handleSubmit();
    //     const formData = new FormData();
    //     formData.append('JSON', JSON.stringify(this.state.fullGoogleResponse))
    //     formData.append('SubFile', this.state.srtFile)
    //     const config = {
    //         headers: {
    //             'content-type': 'application/json'
    //         }
    //     }

    //     axios.post('/api/SavingTranscripts/SaveTranscript', formData, config)
    //         .then(res => {
    //             console.log(res);
    //         })
    //         .catch(err => console.log(err));
    // }

    public onAddAudioFile = (e: any) => {
        this.setState({ audioFile: e.target.files[0] })
    }

    public handleChange = (e: any) => {
        this.setState({ automatedTranscript: e.target.value })
    }

    public handleSearch = (e: any) => {
        this.setState({ searchTerms: e.target.value })
    }

    public editTranscription = () => {
        this.setState({ 'submitEdit': true })
        this.setState({ 'editTranscription': false })
        this.setState({ 'textarea': true })
        this.setState({ 'showAutomatedTranscript': false })
        this.setState({ 'editSuccess': false })
    }

    public handleSubmit = () => {
        this.setState({ 'automatedTranscript': this.state.automatedTranscript })
        this.setState({ 'submitEdit': false })
        this.setState({ 'editTranscription': true })
        this.setState({ 'textarea': false })
        this.setState({ 'showAutomatedTranscript': true })
        this.setState({ 'editSuccess': true })
    }

    public removeEditSuccessMessage = () => {
        this.setState({ 'editSuccess': false })
    }

    public render() {
        const searchForm = <div className="mg-top-30 field is-horizontal">
        <a className="button is-danger mg-right-30" onClick={this.searchTranscript}> Search </a>
        <input className="input" type="text" placeholder="Your search terms" onChange={this.handleSearch} />
        </div>

        const editTranscriptionButton = <a className="button is-danger" onClick={this.editTranscription}>Edit</a>
        const submitEditButton = <a className="button is-danger" >Save</a>

        return (
            <div>
                <div className="container" >

                    <h1 className="title mg-top-30">Google Speech To Text</h1>

                    <div className="level mg-top-30">
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
                        {this.state.loading ? <a className="button is-danger is-loading">Go</a> : <a className="button is-danger" onClick={this.getGoogleSample}>Go</a>}
                    </div>

                    {this.state.editSuccess ? <div className="notification is-success">
                        <button className="delete" onClick={this.removeEditSuccessMessage}></button>
                        You have successfully edited the automated transcription
                        </div> : null}

                    <h3 className="title is-3">{this.state.automatedTranscript == '' ? null : 'Automated transcript'}</h3>

                    {this.state.automatedTranscript == '' ? null : searchForm}
                    {this.state.timestamps == '' ? null : <p> Results : {this.state.timestamps} </p>}

                    <p>{this.state.showAutomatedTranscript ? this.state.automatedTranscript : ''}</p>
                    {this.state.editTranscription ? editTranscriptionButton : null}
                    {this.state.textarea ? <textarea
                        className="textarea"
                        onChange={this.handleChange}
                        rows={6} //Would be nice to adapt this to the number of lines in the future
                        defaultValue={this.state.automatedTranscript}
                        /> : null}
                    {this.state.submitEdit ? submitEditButton : null}
                </div>
            </div>
        );
    }
}
