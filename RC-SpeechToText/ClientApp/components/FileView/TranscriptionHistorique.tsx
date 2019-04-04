import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';

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
                this.setState({ 'versions': res.data.versions});
                this.setState({ 'usernames': res.data.usernames });
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

    public formatTime = (dateModified : string) => {
        var d = new Date(dateModified);


        var datestring = d.getDate() + "-" + (d.getMonth() + 1) + "-" + d.getFullYear() + " " +
            d.getHours() + ":" + d.getMinutes();

        return datestring;

    }

    public render() {
        var i = 0;

        return (
            <div>
                <div className="box mg-top-30" id="historique-title-box">
                      <p className="historique-title"> HISTORIQUE </p>
                </div>
                <div className="box" id="historique-content-box">
                    <div>
                        {this.state.versions.map((version) => {
                            const listVersions = (
                                <div className="historique-content">
                                    <p> {version.historyTitle} </p>
                                    <p> {this.formatTime(version.dateModified)} </p>
                                    <p className="historique-username"> {this.state.usernames[i]}</p>
                                </div>
                            )
                            i++;
                            return listVersions;
                        })}
                     </div>
                </div>
               
            </div>
        );
    }
}
