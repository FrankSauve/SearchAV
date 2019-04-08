import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import { SeeTranscriptionModal } from '../Modals/SeeTranscriptionModal';

interface State {
    loading: boolean,
    unauthorized: boolean, 
    revertTranscription: boolean,
    showTranscriptionModal: boolean,
    revertedVersion: any, 
    currentVersionId: any
}


export class TranscriptionHistorique extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            loading: false,
            unauthorized: false,
            revertTranscription: false,
            showTranscriptionModal: false,
            revertedVersion: null,
            currentVersionId: null 
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

    public showTranscriptionModal = () => {
        this.setState({ showTranscriptionModal: true });
    }

    public hideTranscriptionModal = () => {
        this.setState({ showTranscriptionModal: false });
    }

    public handleHistoryChange = (event: any) => {
        this.setState({ revertTranscription: event.target.value });
    };

    public revertTranscription = (selectedVersion: any) => {
        this.setState({ revertedVersion: selectedVersion });
    };

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
                                    <p> {this.formatTime(version.dateModified)} <a onClick={this.showTranscriptionModal}><i className="fas fa-edit mg-left-5"></i></a> </p>
                                    <p className="historique-username"> <b>{this.props.usernames[i]}</b></p>
                                </div>
                            )
                            i++;
                            this.setState({ currentVersionId: version.id }); 
                            return listVersions;
                        })}
                    </div>
                </div>

                <div>
                    <SeeTranscriptionModal
                        versionId={this.state.currentVersionId}
                        showModal={this.state.showTranscriptionModal}
                        hideModal={this.hideTranscriptionModal}
                        revert={this.state.revertTranscription}
                        handleRevertTranscriptionChange={this.handleHistoryChange}
                        onSubmit={this.revertTranscription}
                    />
                </div> 

            </div>
        );
    }
}