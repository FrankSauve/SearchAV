import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

interface State {
    files: any[],
    userId: AAGUID,
    unauthorized: boolean
}

export default class MyFilesFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            userId: "",
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getUserFiles();
    }

    public getUserFiles = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllFilesByUser/', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.files })
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {   
        return (
            <div className={`card filters mg-top-30 ${this.props.isActive ? "has-background-primary" : "has-background-link"}`}>
                <div className="card-content">
                    <p className="title my-files has-text-white-bis">
                        {this.state.files ? this.state.files.length : 0}
                    </p>
                    <p className="subtitle has-text-white-bis">
                        MES FICHIERS
                </p>
                </div>
            </div>
        )
    }
}