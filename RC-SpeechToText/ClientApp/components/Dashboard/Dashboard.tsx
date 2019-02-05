import * as React from 'react';
import FileInput from './FileInput';
import axios from 'axios';
import auth from '../../Utils/auth';
import FileTable from './FileTable';
import AutomatedFilter from './AutomatedFilter';
import EditedFilter from './EditedFilter';
import ReviewedFilter from './ReviewedFilter';
import MyFilesFilter from './MyFilesFilter';

interface State {
    files: any[],
    usernames: string[],
    userId: number,
    loading: boolean,
    unauthorized: boolean
}

export default class Dashboard extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            files: [],
            usernames: [],
            userId: 0,
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllFiles();
    }

    public getAllFiles = () => {
        this.setState({ 'loading': true });

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/GetAllWithUsernames', config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'files': res.data.value.files });
                this.setState({ 'usernames': res.data.value.usernames })
                this.setState({ 'loading': false });
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public getUserFiles = () => {
        this.setState({ 'loading': true });

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

                axios.get('/api/file/getAllFilesByUser/' + this.state.userId, config)
                    .then(res => {
                        console.log(res);
                        this.setState({ 'files': res.data.value.files })
                        this.setState({ 'usernames': res.data.value.usernames })
                        this.setState({ 'loading': false });
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
            <div className="container">
                <div className="columns">
                    <div className="column is-one-fifth">
                        <FileInput />
                        <br /> <br />
                        <AutomatedFilter />
                        <br />
                        <EditedFilter />
                        <br />
                        <ReviewedFilter />
                        <br /> <br />
                        <a onClick={this.getUserFiles}><MyFilesFilter
                        /></a>
                    </div>
                    <section className="section column">
                        <div className="box">
                            <FileTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                            />
                        </div>
                    </section>
                </div>
            </div>
        )
    }
}
