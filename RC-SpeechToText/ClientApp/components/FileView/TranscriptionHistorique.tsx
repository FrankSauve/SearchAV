import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import Loading from '../Loading';

interface State {
    versions: any[],
    usernames: string[], 
    loading: boolean,
    unauthorized: boolean
}


export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            versions: [],
            usernames: [], 
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
        axios.get('/api/version/GetAllVersionsWithUserName/' + this.props.fileId, config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'versions': res.data.value.versions});
                this.setState({ 'usernames': res.data.value.usernames });
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
        var i = 0;

        return (
            <div>
                <div className="Rectangle-Copy-5">
                      <p className="HISTORIQUE"> HISTORIQUE </p>
                </div>
                <div className="Rectangle-Copy-6">
                    <div className="content_historique">
                        {this.state.versions.map((version) => {
                            const listVersions = <div><p className="title_historique_changes"> {version.historyTitle} </p><p className="historique_date"> {version.dateModified.substr(0, 10)} {version.dateModified.substr(11, 5)} </p><p className="historique_username"> {this.state.usernames[i]}</p> <p> </p></div>
                            i++;
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
