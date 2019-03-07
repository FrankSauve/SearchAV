import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import Loading from '../Loading';
import VersionTable from './VersionTable';

interface State {
    versions: any[],
    loading: boolean,
    unauthorized: boolean
}


export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            versions: [],
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component is rendered
    public componentDidMount() {
        this.getAllVersions();
    }

    public getAllVersions = () => {
        this.setState({ loading: true });


        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/version/GetbyFileId/' + this.props.fileId, config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'versions': res.data });
                this.setState({ 'loading': false })
                console.log(this.state.loading);
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
   };


    public render() {
        return (
            <div>
                <div className="Rectangle-Copy-5">
                      <p className="HISTORIQUE"> HISTORIQUE </p>
                </div>
                <div className="Rectangle-Copy-6">
                    <div className="content_historique">
                        {this.state.versions.map((version) => {
                            const listVersions = <div><p className="title_historique_changes"> MODIFICATIONS </p><p className="historique_date"> {version.dateModified.substr(0, 10)} </p><p className="historique_username"> {version.userId} </p></div>
                            return (
                                listVersions
                            )
                        })}
                     </div>
                </div>
               
            </div>
        );
    }
}
