import * as React from 'react';
import axios from 'axios';


export class TranscriptionSearch extends React.Component<any> {
    constructor(props: any) {
        super(props);
        this.state = {
            searchTerms: '',
            timestamps: '',
        }
    }

    
    public searchTranscript = () => {

    }

    public handleSearch = (e: any) => {
        this.setState({ searchTerms: e.target.value })
    }

    public render() {
        return (
            <div>
                <div className="field is-horizontal">
                    <a className="button is-primary" onClick={this.searchTranscript}> Search </a>
                    <input className="input" type="text" placeholder="Your search terms" onChange={this.handleSearch} />
                </div>
            </div>
        );
    }
}
