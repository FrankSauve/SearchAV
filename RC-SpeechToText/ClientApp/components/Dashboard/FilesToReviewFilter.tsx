import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    files: any[],
    userId: number,
    unauthorized: boolean
}

export default class FilesToReviewFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            userId: 0,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getFilesToReview();
    }

    public getFilesToReview = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/user/getUserByEmail/' + auth.getEmail(), config)
            .then(res => {
                console.log(res);
                this.setState({ 'userId': res.data.id });

                axios.get('/api/file/getUserFilesToReview/' + this.state.userId, config)
                    .then(res => {
                        console.log(res);
                        this.setState({ 'files': res.data.value.files })
                    })
                    .catch(err => {
                        if (err.response.status == 401) {
                            this.setState({ 'unauthorized': true });
                        }
                    });

            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {   
        return (
            <div className={`card filters ${this.props.isActive ? "has-background-primary" : "has-background-link"}`}>
                <div className="card-content">
                    <p className={`title ${this.props.isActive ? "has-text-white-bis" : "has-text-danger"}`}>
                        {this.state.files ? this.state.files.length : 0}
                    </p>
                    <p className={`subtitle ${this.props.isActive ? "has-text-white-bis" : "has-text-danger"}`}>
                        FICHIERS <br/>
                        A REVISER
                </p>
                </div>
            </div>
        )
    }
}