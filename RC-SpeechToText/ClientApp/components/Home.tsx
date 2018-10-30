import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

interface State { 
    automatedTranscript :  string,
    manualTranscript: string,
    accuracy : number
    loading: boolean
 }

export class Home extends React.Component<RouteComponentProps<{}>, State> {

    constructor(props: any){
        super(props);

        this.state = {
            automatedTranscript: '',
            manualTranscript: '',
            accuracy: 0,
            loading: false
        }
        this.getGoogleSample = this.getGoogleSample.bind(this);
    }

    public getGoogleSample() {
        this.setState({'loading': true})
        axios.get('/api/sampletest/googlespeechtotext')
            .then(res => {
                this.setState({'loading': false});
                this.setState({'automatedTranscript': res.data.googleResponse.alternatives[0].transcript});
                this.setState({'manualTranscript': res.data.manualTranscript});
                this.setState({'accuracy': res.data.accuracy});
            })
            .catch(err => console.log(err));
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

    public render() {
        return <div>
            <div className="container">
                <h1>SearchAV</h1>
                <a className="waves-effect waves-light btn red" onClick={this.getGoogleSample}>Google sample</a>
                <br/>
            
                {this.state.loading ? this.loadingCircle : null}
                
                <h3>{this.state.automatedTranscript == '' ? null : 'Automated transcript'}</h3>
                <p>{this.state.automatedTranscript}</p>

                <h3>{this.state.manualTranscript == '' ? null : 'Manual transcript'}</h3>
                <p>{this.state.manualTranscript}</p>

                <h3>{this.state.accuracy == 0 ? null : 'Accuracy'}</h3>
                <p>{this.state.accuracy == 0 ? null : this.state.accuracy}</p>
            </div>
        </div>;
    }
}
