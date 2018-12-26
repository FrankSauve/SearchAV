import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

import {VideoInput} from './VideoInput';
import {VideoInputButton} from './VideoInputButton';
import {TranscriptionText} from './TranscriptionText';

interface State{
    audioFile:any,
    loading:boolean,
    automatedTranscript: string,
    fullGoogleResponse: any,
    showAutomatedTranscript: boolean
}

export default class Transcription extends React.Component<any, State>{
    constructor(props: any) {
        super(props);

        this.state = {
            audioFile: null,
            loading:false,
            automatedTranscript: '',
            fullGoogleResponse: null,
            showAutomatedTranscript: false
        };
    }

    public updateFile = (e: any) => {
        this.setState({audioFile: e.target.files[0]});
        console.log(e.target.files[0]); //TODO: Remove this line
    };
    
    public toggleLoad = (e: any) =>{
        (this.state.loading) ? (this.setState({loading: false})) : (this.setState({loading: true}));
    };
    
    public updateTranscript = (e: any) =>{
        this.setState({ fullGoogleResponse: e });
        this.setState({ automatedTranscript: e.transcript });
        this.setState({ showAutomatedTranscript: true});

        console.log('automatedTranscript after button press: '+this.state.automatedTranscript); //TODO: Remove this line
    };

    public render() {
        
        
        
        return (
            <div className="container">
                <h1 className="title mg-top-30">Transcription</h1>
                <VideoInput 
                    onChange={this.updateFile}
                    audioFile={this.state.audioFile}     
                />
                <br/>
                <VideoInputButton
                    loading={this.state.loading}
                    toggleLoad={this.toggleLoad}
                    audioFile={this.state.audioFile}
                    updateTranscript={this.updateTranscript}
                />
                <br/>
                <TranscriptionText
                    text={this.state.automatedTranscript}
                    showText={this.state.showAutomatedTranscript}
                />
            </div>
        );
    }
}
