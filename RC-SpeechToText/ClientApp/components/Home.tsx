import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import axios from 'axios';

interface State { 
    googleResult :  string,
    loading: boolean
 }

export class Home extends React.Component<RouteComponentProps<{}>, State> {

    constructor(props: any){
        super(props);

        this.state = {
            googleResult: '',
            loading: false
        }
        this.getGoogleSample = this.getGoogleSample.bind(this);
    }

    public getGoogleSample() {
        this.setState({'loading': true})
        axios.get('/api/googletest/speechtotext')
            .then(res => {
                this.setState({'loading': false})
                this.setState({'googleResult': JSON.stringify(res.data)})
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

                <p>{this.state.googleResult}</p>
            </div>
        </div>;
    }
}
