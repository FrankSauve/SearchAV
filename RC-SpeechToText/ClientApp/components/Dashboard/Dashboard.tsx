import * as React from 'react';
import FileInput from './FileInput';
import axios from 'axios';
import auth from '../../Utils/auth';
import GridFileTable from './Grid/GridFileTable';
import AutomatedFilter from './filters/AutomatedFilter';
import EditedFilter from './filters/EditedFilter';
import ReviewedFilter from './filters/ReviewedFilter';
import MyFilesFilter from './filters/MyFilesFilter';
import Loading from '../Loading';
import FilesToReviewFilter from './filters/FilesToReviewFilter';
import ListTable from './list/ListTable';

interface State {
    files: any[],
    usernames: string[],
    userId: AAGUID,
    isMyFilesFilterActive: boolean,
    isEditedFilterActive: boolean,
    isAutomatedFilterActive: boolean,
    isReviewedFilterActive: boolean,
    searchTerms: string,
    isFilesToReviewFilterActive: boolean,
    listView: boolean,
    loading: boolean,
    unauthorized: boolean
}

export default class Dashboard extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            files: [],
            usernames: [],
            userId: "",
            isMyFilesFilterActive: false,
            isEditedFilterActive: false,
            isAutomatedFilterActive: false,
            isReviewedFilterActive: false,
            isFilesToReviewFilterActive: false,
            listView: false,
            searchTerms: '',
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
        this.deactivateFilters();


        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/GetAllWithUsernames', config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'files': res.data.files });
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ 'loading': false });
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
        this.deactivateFilters();

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
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isMyFilesFilterActive': true });
                this.setState({ loading: false });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getUserFilesToReview = () => {
        this.setState({ loading: true });
        this.deactivateFilters();

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
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ loading: false });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getAutomatedFiles = () => {
        this.setState({ loading: true });
        this.deactivateFilters();

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllFilesByFlag/Automatise', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.files })
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isAutomatedFilterActive': true });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getEditedFiles = () => {
        this.setState({ loading: true });
        this.deactivateFilters();

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllFilesByFlag/Edite', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.files })
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isEditedFilterActive': true });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public getReviewedFiles = () => {
        this.setState({ loading: true });
        this.deactivateFilters();

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
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ 'loading': false });
                this.setState({ 'isReviewedFilterActive': true });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public renderFileTable = () => {
        this.setState({ 'listView': false });
        return (
            <div>
                {this.state.files ? <GridFileTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                                getAllFiles={this.getAllFiles}
                            /> : <h1 className="title">NO FILES</h1>}
           </div>
        )
    }

    public renderListView = () => {
        this.setState({ 'listView': true });
        return (
            <div>
                {this.state.files ? <ListTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                                getAllFiles={this.getAllFiles}
                            /> : <h1 className="title">NO FILES</h1>}
            </div>
        )
    }

    public searchDescription = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/getFilesByDescriptionAndTitle/' + this.state.searchTerms, config)
            .then(res => {
                this.setState({ files: res.data });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public handleSearch = (e: any) => {
        this.setState({ searchTerms: e.target.value })
    }

    public deactivateFilters = () => {
        this.setState({ 'isMyFilesFilterActive': false });
        this.setState({ 'isAutomatedFilterActive': false });
        this.setState({ 'isEditedFilterActive': false });
        this.setState({ 'isReviewedFilterActive': false });
        this.setState({ 'isFilesToReviewFilterActive': false });
    }

    public render() {
        return (
            <div className="container">
                <div className="columns">
                    <div className="column is-one-fifth">
                        <FileInput
                            getAllFiles={this.getAllFiles}
                            getAutomatedFiles={this.getAutomatedFiles}
                        />

                        <br /> <br />

                        <a onClick={!this.state.isFilesToReviewFilterActive ? this.getUserFilesToReview : this.getAllFiles}>
                            <FilesToReviewFilter
                                isActive={this.state.isFilesToReviewFilterActive}
                            />
                        </a>

                        <br />

                        <a onClick={!this.state.isAutomatedFilterActive ? this.getAutomatedFiles : this.getAllFiles}>
                            <AutomatedFilter
                                isActive={this.state.isAutomatedFilterActive}
                            />
                        </a>

                        <br />

                        <a onClick={!this.state.isEditedFilterActive ? this.getEditedFiles : this.getAllFiles}>
                            <EditedFilter
                                isActive={this.state.isEditedFilterActive}
                            />
                        </a>

                        <br />

                        <a onClick={!this.state.isReviewedFilterActive ? this.getReviewedFiles : this.getAllFiles}>
                            <ReviewedFilter
                                isActive={this.state.isReviewedFilterActive}
                            />
                        </a>

                        <br /> <br />

                        <a onClick={!this.state.isMyFilesFilterActive ? this.getUserFiles : this.getAllFiles}>
                            <MyFilesFilter
                                isActive={this.state.isMyFilesFilterActive}
                            />
                        </a>
                    </div>

                    <section className="section column">
                        <div>
                            <div className="field is-horizontal">
                                <a className="button is-link mg-right-10" onClick={this.searchDescription}> Rechercher </a>
                                <input className="input" type="text" placeholder="Chercher les fichiers par titre ou description" onChange={this.handleSearch} />
                                &nbsp;
                                <a>< img src="assets/grid.png" onClick={this.renderFileTable} width="40" height="40" /></a>
                                &nbsp;
                                <a><img src="assets/list.png" onClick={this.renderListView} width="40" height="40" /></a>
                            </div>
                        </div>

                        <div className="box file-box mg-top-30">
                            {this.state.loading ? <Loading /> :
                                this.state.listView ? this.renderListView() : this.renderFileTable()}
                        </div>
                    </section>

                </div>
            </div>
        )
    }
}
