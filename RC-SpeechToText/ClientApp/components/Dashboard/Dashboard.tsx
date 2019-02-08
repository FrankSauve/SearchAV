import * as React from 'react';
import FileInput from './FileInput';
import axios from 'axios';
import auth from '../../Utils/auth';
import FileTable from './FileTable';
import AutomatedFilter from './AutomatedFilter';
import EditedFilter from './EditedFilter';
import ReviewedFilter from './ReviewedFilter';
import MyFilesFilter from './MyFilesFilter';
import { FileDescriptionSearch } from '../FileView/FileDescriptionSearch';
import Loading from '../Loading';

interface State {
    files: any[],
    usernames: string[],
    userId: number,
    isMyFilesFilterActive: boolean,
    isEditedFilterActive: boolean,
    isAutomatedFilterActive: boolean,
    isReviewedFilterActive: boolean,
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
            isMyFilesFilterActive: false,
            isEditedFilterActive: false,
            isAutomatedFilterActive: false,
            isReviewedFilterActive: false,
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllFiles();
    }

    public getAllFiles = () => {
        this.setState({ loading: true });


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
                this.setState({ 'isMyFilesFilterActive': false });
                this.setState({ 'isAutomatedFilterActive': false });
                this.setState({ 'isEditedFilterActive': false });
                this.setState({ 'isReviewedFilterActive': false });
                this.setState({ loading: false });
                console.log(this.state.loading);
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public getUserFiles = () => {
        this.setState({ loading: true });

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
                        this.setState({ 'isMyFilesFilterActive': true });
                        this.setState({ 'isAutomatedFilterActive': false });
                        this.setState({ 'isEditedFilterActive': false });
                        this.setState({ 'isReviewedFilterActive': false });
                        this.setState({ loading: false });
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

    public getAutomatedFiles = () => {
        this.setState({ loading: true });

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllAutomatedFiles', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.value.files })
                this.setState({ 'usernames': res.data.value.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isMyFilesFilterActive': false });
                this.setState({ 'isAutomatedFilterActive': true });
                this.setState({ 'isEditedFilterActive': false });
                this.setState({ 'isReviewedFilterActive': false });
                this.setState({ loading: false });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getReviewedFiles = () => {
        this.setState({ loading: true });

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllReviewedFiles', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.value.files })
                this.setState({ 'usernames': res.data.value.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isMyFilesFilterActive': false });
                this.setState({ 'isAutomatedFilterActive': false });
                this.setState({ 'isEditedFilterActive': false });
                this.setState({ 'isReviewedFilterActive': true });
                this.setState({ loading: false });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getEditedFiles = () => {
        this.setState({ loading: true });

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllEditedFiles', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.value.files })
                this.setState({ 'usernames': res.data.value.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isMyFilesFilterActive': false });
                this.setState({ 'isAutomatedFilterActive': false });
                this.setState({ 'isEditedFilterActive': true });
                this.setState({ 'isReviewedFilterActive': false });
                this.setState({ loading: false });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public renderFileTable = () => {
        return (
            <div>
                {this.state.files ? <FileTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                            /> : <h1 className="title">NO FILES</h1>}
           </div>
        )
    }

    public render() {
        return (
            <div className="container">
                <div className="columns">
                    <div className="column is-one-fifth">
                        <FileInput />

                        <br /> <br />

                        <a onClick={this.getAutomatedFiles}>
                            <AutomatedFilter
                                isActive={this.state.isAutomatedFilterActive}
                            />
                        </a>

                        <br />

                        <a onClick={this.getEditedFiles}>
                            <EditedFilter
                                isActive={this.state.isEditedFilterActive}
                            />
                        </a>

                        <br />

                        <a onClick={this.getReviewedFiles}>
                            <ReviewedFilter
                                isActive={this.state.isReviewedFilterActive}
                            />
                        </a>

                        <br /> <br />

                        <a onClick={this.getUserFiles}>
                            <MyFilesFilter
                                isActive={this.state.isMyFilesFilterActive}
                            />
                        </a>
                    </div>

                    <section className="section column">
                        <FileDescriptionSearch />
                        <div className="box">
                            {this.state.loading ? <Loading /> : this.state.files ? <FileTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                            /> : <h1 className="title">NO FILES</h1> }
                        </div>
                    </section>

                </div>
            </div>
        )
    }
}
