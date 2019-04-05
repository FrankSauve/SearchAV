import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';

interface State {
    loading: boolean,
    unauthorized: boolean
}


export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component is rendered
    public componentDidMount() {

    }

    public formatTime = (dateModified: string) => {
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
                        {this.props.versions.map((version: any) => {
                            const listVersions = (
                                <div className="historique-content">
                                    <p> {version.historyTitle} </p>
                                    <p> {this.formatTime(version.dateModified)} </p>
                                    <p className="historique-username"> {this.props.usernames[i]}</p>
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