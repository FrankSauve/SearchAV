import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

interface State {
    files: any[],
    showArrow: boolean,
    unauthorized: boolean
}

export default class ReviewedFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            showArrow: false,
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

        axios.get('/api/file/Revise', config)
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
        this.setState({ showArrow: true });
    }

    public handleLeave = () => {
        this.setState({ showArrow: false });
    }

    public render() {   
        return (
            <a><div className={`card filters mg-top-5 ${this.props.isActive ? "has-background-blizzard-blue" : "has-background-link"}`}>
                <div className="card-content" onMouseEnter={this.handleHover} onMouseLeave={this.handleLeave}>
                    <p className={`title ${this.props.isActive ? "is-link" : "reviewed" }`}>
                        {this.state.files.length}
                    </p>
                    <p className={`subtitle ${this.props.isActive ? "is-link" : "reviewed"}`}>
                        {this.props.isActive ? <i className="fas fa-arrow-left"></i> : this.state.showArrow ? <i className="fas fa-arrow-right right reviewed"></i> : null}
                        <b>FICHIERS<br />
                            REVISES</b></p>
                </div>
            </div></a>
        )
    }
}