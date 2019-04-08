import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import { TranscriptionHistoriqueItem } from '../FileView/TranscriptionHistoriqueItem'

interface State {
    loading: boolean,
    unauthorized: boolean, 
    activeVersion: any, 
    currentTranscription: any
}


export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            loading: false,
            unauthorized: false, 
            activeVersion: this.props.activeVersion, 
            currentTranscription: this.props.currentTranscription
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
                            const VersionComponent = (
                                <TranscriptionHistoriqueItem
                                    version={version}
                                    historyTitle={version.historyTitle}
                                    username={this.props.usernames[i]}
                                    dateModified={this.formatTime(version.dateModified)}
                                    transcription={version.transcription}
                                    activeVersion={this.state.activeVersion.id}
                                    currentTranscription={this.state.currentTranscription}
                                />
                            )
                            i++;
                            return VersionComponent;
                        })}
                    </div>
                </div>
            </div>
        );
    }
}