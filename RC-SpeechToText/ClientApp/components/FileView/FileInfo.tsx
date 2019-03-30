import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { ThumbnailSelectionModal } from '../Modals/ThumbnailSelectionModal';

interface State {
    thumbnail: string,
    name: string,
    userId: AAGUID,
    showThumbnailModal: boolean,
    dateModified: string,
    dateFormated: string,
    unauthorized: boolean

}

export class FileInfo extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            thumbnail: this.props.thumbnail,
            name: '',
            userId: this.props.userId,
            showThumbnailModal: false,
            dateModified: this.props.dateModified,
            dateFormated: '',
            unauthorized: false
        }
    }

    // Called when the component is rendered
    public componentDidMount() {
        this.getUserFile();
        this.formatTime();
    }

    public showThumbnailModalModal = () => {
        this.setState({ showThumbnailModal: true });
    };

    public hideThumbnailModal = () => {
        this.setState({ showThumbnailModal: false });
    };

    public getUserFile = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/User/GetUserName/' + this.state.userId + '/', config)
            .then(res => {
                this.setState({ name: res.data.name });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public formatTime = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/File/FormatTime/' + this.state.dateModified + '/', config)
            .then(res => {
                this.setState({ dateFormated: res.data });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {
        return (
            <div className = "columns">
                <div className="column">
                    {<b className="file-view-header">Image associ&#233;e: <a onClick={this.showThumbnailModalModal}><i className="fas fa-edit mg-left-5"></i></a> </b>}

                    <figure className="image file-view-image">
                        <img src={this.state.thumbnail} alt="Fichier vid&#233;o n'a pas d'image associ&#233;e" />                       
                    </figure>
                </div>
                <div className="column">
                    <b className="file-view-header">Date de modification: </b>
                    <p className="file-view-info"> {this.state.dateFormated} </p><br />
                    <b className="file-view-header">Import&#233; par: </b>
                    <p className="file-view-info">{this.state.name}</p>
                </div>

                <ThumbnailSelectionModal
                    showModal={this.state.showThumbnailModal}
                    hideModal={this.hideThumbnailModal}
                    file={this.props.file}
                />

            </div>
        );
    }
}