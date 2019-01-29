import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Redirect } from 'react-router-dom';

import {VideoInput} from './VideoInput';
import {TranscriptionButton} from './TranscriptionButton';
import {EditTranscriptionButton} from './EditTranscriptionButton';
import {SearchField} from './SearchField';
import { TranscriptionText } from './TranscriptionText';
//import  FileTable  from '../MyFiles/FileTable';
import File from '../MyFiles/File';

import {VideoView} from '../VideoPlayer/VideoView';

interface State{
    audioFile: any,
    files: any[],
    loading: boolean,
    loadingFiles: boolean,
    automatedTranscript: string,
    showEditButton: boolean,
    editedTranscription: string,
    searchTerms: string,
    timestamps: string,
    fullGoogleResponse: any,
    showAutomatedTranscript: boolean,
    showVideo: boolean,
    isEditing: boolean,
    versionId: number,
    unauthorized: boolean 
}

export default class Dashboard extends React.Component<any, State>{
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null,
            files: [],
            loading: false,
            loadingFiles: false,
            automatedTranscript: '',
            editedTranscription: '',
            searchTerms: '',
            timestamps: '',
            fullGoogleResponse: null,
            showAutomatedTranscript: false,
            showEditButton: false,
            showVideo: false,
            isEditing: false,
            versionId: 0,
            unauthorized: false
        };
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllFiles();
    }

    public getAllFiles = () => {
        this.setState({ 'loadingFiles': true });

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/index', config)
            .then(res => {
                this.setState({ 'files': res.data });
                this.setState({ 'loadingFiles': false });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public updateFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]});
        console.log(e.target.files[0]);
    };

    public handleEditChange = (e: any) => {
        this.setState({ editedTranscription: e.target.value })
    }

    public editTranscription = () =>  {
        this.setState({ isEditing: true });
    }

    public handleSubmit = () => {
        this.setState({ automatedTranscript: this.state.editedTranscription })
        this.setState({ 'isEditing': false })
    }

    public handleCancel = () => {
        this.setState({ 'isEditing': false })
    }
    
    public toggleLoad = (e: any) =>{
        (this.state.loading) ? (this.setState({ loading: false })) : (this.setState({ loading: true }));
    };
    
    public updateTranscript = (transcription: any) =>{
        this.setState({ automatedTranscript: transcription });
        this.setState({ showAutomatedTranscript: true});
        this.setState({ showVideo: true });
        this.setState({ showEditButton: true });
    };

    public updateVersionId = (Id: any) => {
        this.setState({ versionId: Id });
    };

    public updateSearchTerms = (e: any) =>{
        this.setState({ searchTerms: e.target.value });
    };

    public searchTranscript = () => {
        // TODO: Change all this
        const formData = new FormData();
        formData.append('searchTerms', this.state.searchTerms);
        formData.append('jsonResponse', JSON.stringify(this.state.fullGoogleResponse));

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.post('/api/TranscriptSearch/SearchTranscript', formData, config)
            .then(res => {
                this.setState({ 'timestamps': res.data });
            })
            .catch(err => {
                if(err.response.status == 401) {
                    this.setState({'unauthorized': true});
                }
            });
    };

    public render() {

        const progressBar = <img src="assets/loading.gif" alt="Loading..." />

        return (
            <div className="container">

                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}

                <h1 className="title mg-top-30">Transcription</h1>
                <VideoInput 
                    onChange={this.updateFile}
                    audioFile={this.state.audioFile}     
                />
                <br/>
                <TranscriptionButton
                    loading={this.state.loading}
                    toggleLoad={this.toggleLoad}
                    audioFile={this.state.audioFile}
                    updateTranscript={this.updateTranscript}
                    updateVersionId={this.updateVersionId}
                />
                
                <SearchField
                    searchTranscript={this.searchTranscript}
                    updateSearchTerms={this.updateSearchTerms}
                    timestamps={this.state.timestamps}
                    showText={this.state.showAutomatedTranscript}
                />
                <TranscriptionText
                    text={this.state.automatedTranscript}
                    showText={this.state.showAutomatedTranscript}
                    isEditing={this.state.isEditing}
                    handleEditChange={this.handleEditChange}
                />
                <EditTranscriptionButton
                    showText={this.state.showAutomatedTranscript}
                    showEditButton={this.state.showEditButton}
                    isEditing={this.state.isEditing}
                    editTranscription={this.editTranscription}
                    editedTranscription={this.state.editedTranscription}
                    transcription={this.state.automatedTranscript}
                    versionId={this.state.versionId}
                    updateVersionId={this.updateVersionId}
                    handleSubmit={this.handleSubmit}
                />
                <VideoView
                    audioFile={this.state.audioFile}
                    showVideo={this.state.showVideo}
                />

                {
                    //TO DO: Refactor this to use FileTable component
                }
                <div className="container has-text-centered">

                    {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}

                    {this.state.loadingFiles ? progressBar : null}
                    <div className="columns is-multiline">
                        {this.state.files.map((file) => {
                            return (
                                <File
                                    fileId={file.id}
                                    flag={file.flag}
                                    title={file.title}
                                    filePath={file.filePath}
                                    transcription={file.transcription}
                                    dateAdded={file.dateAdded}
                                    key={file.id}
                                />
                            )
                        })}
                    </div>
                </div>

                {
                    //TODO: Update the video when audioFile is changed
                }
                
                
                <br/><br/><br/>
            </div>
        );
    }
}
