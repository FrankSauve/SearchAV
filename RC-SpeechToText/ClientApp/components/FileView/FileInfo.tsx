import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    thumbnail: string,
    name: string,
    userId: string,
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
                    {<b>Image associ&#233;e: </b>}

                    <figure className="image is-4by3">
                        <img src={this.state.thumbnail} alt="Fichier vid&#233;o n'a pas d'image associ&#233;e" />                       
                    </figure>
                </div>
                <div className="column">
                    <b>Date de modification: </b>
                    <p> {this.state.dateFormated} </p>
                    <b>Import&#233; par: </b>
                    <p>{this.state.name}</p>

                </div>

            </div>
        );
    }
}