import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    files: any[],
    unauthorized: boolean
}

export default class EditedFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getEditedFiles();
    }

    public getEditedFiles = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllEditedFiles', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data })
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {   
        return (
            <a><div className="card filters has-background-link">
                <div className="card-content">
                    <p className="title has-text-primary">
                        {this.state.files.length}
                    </p>
                    <p className="subtitle has-text-primary">
                        FICHIERS<br />
                        EDITES
                </p>
                </div>
            </div></a>
        )
    }
}