import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    searchTerms: string
    unauthorized: boolean
    files: any
}

export class FileDescriptionSearch extends React.Component<any,State> {
    constructor(props: any) {
        super(props);

        this.state = {
            searchTerms: '',
            unauthorized: false,
            files: null
        }
    }
    
    public searchDescription = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/getFilesByDescription/' + this.state.searchTerms , config)
            .then(res => {
                this.setState({ files: res.data });
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
                    <a className="button is-link mg-right-10" onClick={this.searchDescription}> Rechercher </a>
                    <input className="input" type="text" placeholder="Your search terms" onChange={this.handleSearch} />
                </div>
                {this.state.files ? <p> Résultats : {this.state.files} </p> : null}
            </div>
        );
    }
}
