import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Redirect } from 'react-router-dom';

import {VideoInput} from './VideoInput';
import {TranscriptionButton} from './TranscriptionButton';
import {EditTranscriptionButton} from './EditTranscriptionButton';
import {SearchField} from './SearchField';
import {TranscriptionText} from './TranscriptionText';

import {VideoView} from '../VideoPlayer/VideoView';

interface State{
    audioFile:any,
    loading:boolean,
    automatedTranscript: string,
    searchTerms: string,
    timestamps: string,
    fullGoogleResponse: any,
    showAutomatedTranscript: boolean,
    showVideo: boolean,
    isEditing: boolean,
    unauthorized: boolean 
}

export default class Transcription extends React.Component<any, State>{
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null,
            loading:false,
            automatedTranscript: '',
            searchTerms: '',
            timestamps: '',
            fullGoogleResponse: null,
            showAutomatedTranscript: false,
            showVideo: false,
            isEditing: false,
            unauthorized: false
        };
    }

    public updateFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]});
        console.log(e.target.files[0]);
    };

    public handleEditChange = (e: any) => {
        this.setState({ automatedTranscript: e.target.value })
    }

    public editTranscription = () =>  {
        this.setState({ isEditing: true });
    }

    public saveTranscription = () => {
        alert("Feature not implemented yet");
        // TODO: We need the ID of the video.
        // We probably need to change the object that gets returned by /convertandtranscribe to include the ID of the video
    }
    
    public toggleLoad = (e: any) =>{
        (this.state.loading) ? (this.setState({loading: false})) : (this.setState({loading: true}));
    };
    
    public updateTranscript = (transcription: any) =>{
        this.setState({ automatedTranscript: transcription });
        this.setState({ showAutomatedTranscript: true});
        this.setState({ showVideo: true});
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
                    isEditing={this.state.isEditing}
                    editTranscription={this.editTranscription}
                    saveTranscription={this.saveTranscription}
                />
                <VideoView
                    audioFile={this.state.audioFile}
                    showVideo={this.state.showVideo}
                />
                {
                    //TODO: Update the video when audioFile is changed
                }
                
                
                <br/><br/><br/>
            </div>
        );
    }
}
