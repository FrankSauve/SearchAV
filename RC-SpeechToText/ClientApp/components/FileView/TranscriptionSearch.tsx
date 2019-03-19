import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    timestamps: any,
    searchTerms: string,
    versionId: AAGUID,
    unauthorized: boolean
}

export class TranscriptionSearch extends React.Component<any,State> {
    constructor(props: any) {
        super(props);

        this.state = {
            versionId: this.props.versionId,
            searchTerms: '',
            timestamps: null,
            unauthorized: false
        }
    }

    
    public searchTranscript = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/transcription/SearchTranscript/' + this.state.versionId + '/' + this.state.searchTerms , config)
            .then(res => {
                this.setState({ timestamps: res.data });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public handleSearch = (e: any) => {
        this.setState({ searchTerms: e.target.value })
    }

    public render() {
        return (
            <div>
                <div className="field is-horizontal">
                    <a className="button is-link mg-right-10" onClick={this.searchTranscript}> Rechercher </a>
                    <input className="input" type="text" placeholder="Your search terms" onChange={this.handleSearch} />
                </div>
                {this.state.timestamps ?  <p> Résultats : {this.state.timestamps} </p> : null}
            </div>
        );
    }
}
