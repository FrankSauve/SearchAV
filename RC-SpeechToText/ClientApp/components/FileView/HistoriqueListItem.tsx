import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';

interface State {
    version: any,
    unauthorized: boolean
}


export class HistoriqueListItem extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            version: this.props.version,
            unauthorized: false
        }
    }

    // Called when the component is rendered
    public componentDidMount() {

    }

    public formatTime = (dateModified: string) => {
        var d = new Date(dateModified);

        var day = d.getDate() < 10 ? "0" + d.getDate() : d.getDate();
        var month = d.getMonth() < 10 ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1);
        var hours = d.getHours() < 10 ? "0" + d.getHours() : d.getHours();
        var minutes = d.getMinutes() < 10 ? "0" + d.getMinutes() : d.getMinutes();

        var datestring = day + "-" + month + "-" + d.getFullYear() + " " + hours + ":" + minutes;

        return datestring;

    }

    public handleOnClickComponent = () => {
        if (this.props.versionToDiff != null && this.props.versionToDiff.id == this.state.version.id) {
            this.props.updateDiff(null);
        } else {
            this.props.updateDiff(this.state.version);
        }
    }

    public render() {
        var i = 0;

        return (
            <div className="historique-content" onClick={this.handleOnClickComponent}>
                <p> {this.state.version.historyTitle} </p>
                <p> {this.formatTime(this.state.version.dateModified)} </p>
                <p className="historique-username"> <b>{this.props.username}</b></p>
            </div>
        );
    }
}