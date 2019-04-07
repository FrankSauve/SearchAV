import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

interface State {
    files: any[],
    userId: AAGUID,
    showArrow: boolean,
    unauthorized: boolean
}

export default class FilesToReviewFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            userId: "",
            showArrow: false,
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

        axios.get('/api/file/getUserFilesToReview/', config)
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

    public handleHover = () => {
        if (!this.props.isActive)
            this.setState({ showArrow: true });
    }

    public handleLeave = () => {
        if (!this.props.isActive)
            this.setState({ showArrow: false });
    }

    public render() {
        return (
            <div className={`card filters mg-top-30 ${this.props.isActive ? "has-background-blizzard-blue" : "has-background-link"}`}>
                <div className="card-content" onMouseEnter={this.handleHover} onMouseLeave={this.handleLeave}>
                    <p className={`title ${this.props.isActive ? "is-link" : "has-text-danger"}`}>
                        {this.state.files ? this.state.files.length : 0}
                    </p>
                    <p className={`subtitle ${this.props.isActive ? "is-link" : "has-text-danger"}`}>
                        {this.state.showArrow ? <i className="fas fa-arrow-right is-danger"></i> : null}
                        <b>FICHIERS <br />
                            A REVISER</b>
                </p>
                </div>
            </div>
        )
    }
}