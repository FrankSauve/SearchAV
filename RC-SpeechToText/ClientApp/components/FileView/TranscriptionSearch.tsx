import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
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
            unauthorized: false
        }
    }
    
    public handleChange = (e: any) =>{
        this.setState({searchTerms: e.target.value});
    };

    public handleSearch = () => {
        let selectedWords = this.state.searchTerms ? this.state.searchTerms.toString().split(" ") : null;

        if (selectedWords && this.state.searchTerms && this.state.searchTerms!='') {
            this.props.handleSelectionChange(this.state.searchTerms.toString());
        }
    };

    public handleKeyPress = (e: any) => {
        if (e.key === 'Enter') {
            this.handleSearch();
        }
    };

    public render() {
        return (
            <div className="search-div-file-view">
                <div className="field is-horizontal mg-top-10">
                    <p className="is-cadet-grey search-title">TRANSCRIPTION</p>
                    <div className="right-side">
                        <div className="search-field-file-view">
                            <p className="control has-icons-right">
                                <input className="input is-rounded search-input" type="search" onChange={this.handleChange} onKeyPress={this.handleKeyPress} />
                                <span className="icon is-small is-right">
                                    <a onClick={this.handleSearch}><i className="fas fa-search is-cadet-grey"></i></a>
                                </span>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
