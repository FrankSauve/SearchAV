import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    files: any[],
    unauthorized: boolean
}

export default class ReviewedFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAutomatedFiles();
    }

    public getAutomatedFiles = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllAutomatedFiles', config)
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
            <a><div className="card filters">
                <div className="card-content">
                    <p className="title">
                        {this.state.files.length}
                    </p>
                    <p className="subtitle">
                        FICHIERS<br />
                        REVISES</p>
                </div>
            </div></a>
        )
    }
}