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
    
    public handleChange = (e: any) =>{
        this.setState({searchTerms: e.target.value});
    };

    public handleSearch = () => {
        if(this.state.searchTerms!='' && this.state.searchTerms != null){
            this.props.handleSelectionChange(this.state.searchTerms);
            let a = this.props.searchTranscript(this.state.searchTerms, true);
            this.setState({timestamps: a});
        }
    };

    public render() {
        return (
            <div>
                <div className="field is-horizontal">
                    <a className="button is-link mg-right-10" onClick={this.handleSearch}> Rechercher </a>
                    <input className="input" type="text" placeholder="Your search terms" onChange={this.handleChange} />
                </div>
                {this.state.timestamps ?  <p> Résultats : {this.state.timestamps} </p> : null}
            </div>
        );
    }
}
