import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

interface State {
    files: any[],
    userId: AAGUID,
    showArrow: boolean,
    unauthorized: boolean
}

export default class MyFilesFilter extends React.Component<any, State> {

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

    public handleHover = () => {
        this.setState({ showArrow: true });
    }

    public handleLeave = () => {
        this.setState({ showArrow: false });
    }


    public render() {   
        return (
            <div className={`card filters mg-top-30 ${this.props.isActive ? "has-background-blizzard-blue" : "has-background-link"}`}>
                <div className="card-content" onMouseEnter={this.handleHover} onMouseLeave={this.handleLeave}>
                    <p className={`title ${this.props.isActive ? "is-link" : "is-white-smoke"}`}>
                        {this.state.files ? this.state.files.length : 0}
                    </p>
                    <p className={`subtitle ${this.props.isActive ? "is-link" : "is-white-smoke"}`}>
                        {this.state.showArrow ? <i className="fas fa-arrow-right is-white"></i> : null}
                        <b>MES FICHIERS</b>
                </p>
                </div>
            </div>
        )
    }
}