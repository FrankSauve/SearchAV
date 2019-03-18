import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

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
        this.getReviewedFiles();
    }

    public getReviewedFiles = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllFilesByFlag/Revise', config)
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
            <a><div className={`card filters ${this.props.isActive ? "has-background-primary" : "has-background-link"}`}>
                <div className="card-content">
                    <p className={`title ${this.props.isActive ? "has-text-white-bis" : "has-text-success"}`}>
                        {this.state.files.length}
                    </p>
                    <p className={`subtitle ${this.props.isActive ? "has-text-white-bis" : "has-text-success"}`}>
                        FICHIERS<br />
                        REVISES</p>
                </div>
            </div></a>
        )
    }
}