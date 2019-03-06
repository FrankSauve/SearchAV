import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import Loading from '../Loading';
import VersionTable from './VersionTable';

interface State {
    versions: any[],
    usernames: string[],
    dates: string[],
    loading: boolean,
    unauthorized: boolean
}

export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            versions: [],
            usernames: [],
            dates: [],
            loading: false,
            unauthorized: false
        }
    }

    public getAllVersions = () => {
        this.setState({ loading: true });


        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/versions/GetbyFileId', config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'versions': res.data.value.versions });
                this.setState({ 'loading': false });
                this.setState({ 'usernames': res.data.value.usernames });
                this.setState({ 'dates': res.data.value.dates });
                this.setState({ loading: false });
                console.log(this.state.loading);
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public renderVersionsTable = () => {
        return (
            <div>
                {this.state.versions ? <VersionTable
                    versions={this.state.versions}
                    usernames={this.state.usernames}
                    dates={this.state.dates}
                    loading={this.state.loading}
                /> : <h1 className="title">NO Versions</h1>}
            </div>
        )
    }

    public render() {
        return (
            <div>
                <div className="Rectangle-Copy-5">
                      <p className="HISTORIQUE"> HISTORIQUE </p>
                </div>
                <div className="Rectangle-Copy-6">
                     <div className="content_historique">
                            <p className="title_historique_changes"> MODIFICATIONS </p>
                            <p className="historique_date"> 29-10-2018 11:45 </p>
                            <p className="historique_username"> John Doe </p>
                     </div>
                </div>
                <div className="box mg-top-30">
                    {this.state.loading ? <Loading /> : this.state.versions ? <VersionTable
                        versions={this.state.versions}
                        usernames={this.state.usernames}
                        dates={this.state.dates}
                        loading={this.state.loading}
                    /> : <h1 className="title">NO VERSIONS</h1>}
                </div>
            </div>
        );
    }
}
