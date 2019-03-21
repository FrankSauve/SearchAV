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
    userId: any,
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
                this.setState({ 'isFilesToReviewFilterActive': true });
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

    public showFileTable = () => {
        this.setState({ 'listView': false });
    }
    
    public renderFileTable = () => {        
        return (
            <div>
                {this.state.files.length > 0 ? <GridFileTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                                getAllFiles={this.getAllFiles}
                        /> : <h1 className="title no-files">AUCUN FICHIERS</h1>}
           </div>
        )
    }

    public showListView = () => {
        this.setState({ 'listView': true });
    }
    public renderListView = () => {
        return (
            <div>
                {this.state.files.length > 0 ? <ListTable
                                files={this.state.files}
                                usernames={this.state.usernames}
                                loading={this.state.loading}
                                getAllFiles={this.getAllFiles}
                            /> : <h1 className="title no-files">AUCUN FICHIERS</h1>}
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

    public handleKeyPress = (e: any) => {
        if (e.key === 'Enter') {
            this.searchDescription();
        }
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

        var fileType = this.state.isAutomatedFilterActive ? "TRANSCRITS" : this.state.isEditedFilterActive ? "EDITES" : this.state.isReviewedFilterActive ? "REVISES" : this.state.isFilesToReviewFilterActive ? "A REVISER" : "" 

        return (
            <div className="container">
                <div className="columns">
                    <div className="column is-one-fifth">
                        <FileInput
                            getAllFiles={this.getAllFiles}
                            getAutomatedFiles={this.getAutomatedFiles}
                        />

                        <a onClick={!this.state.isFilesToReviewFilterActive ? this.getUserFilesToReview : this.getAllFiles}>
                            <FilesToReviewFilter
                                isActive={this.state.isFilesToReviewFilterActive}
                            />
                        </a>
                        
                        <a onClick={!this.state.isAutomatedFilterActive ? this.getAutomatedFiles : this.getAllFiles}>
                            <AutomatedFilter
                                isActive={this.state.isAutomatedFilterActive}
                            />
                        </a>

                        <a onClick={!this.state.isEditedFilterActive ? this.getEditedFiles : this.getAllFiles}>
                            <EditedFilter
                                isActive={this.state.isEditedFilterActive}
                            />
                        </a>

                        <a onClick={!this.state.isReviewedFilterActive ? this.getReviewedFiles : this.getAllFiles}>
                            <ReviewedFilter
                                isActive={this.state.isReviewedFilterActive}
                            />
                        </a>

                        <a onClick={!this.state.isMyFilesFilterActive ? this.getUserFiles : this.getAllFiles}>
                            <MyFilesFilter
                                isActive={this.state.isMyFilesFilterActive}
                            />
                        </a>
                    </div>

                    <section className="section column">
                        <div className="search-div">
                            <div className="field is-horizontal mg-top-10">
                                <p className="is-cadet-grey search-title">{this.state.isMyFilesFilterActive ? "MES " : ""} FICHIERS {fileType}</p>
                                <div className="search-field">
                                    <p className="control has-icons-right">
                                        <input className="input is-rounded search-input" type="text" onChange={this.handleSearch} onKeyPress={this.handleKeyPress}/>
                                        <span className="icon is-small is-right">
                                            <a onClick={this.searchDescription}><i className="fas fa-search is-cadet-grey"></i></a>
                                        </span>
                                    </p>
                                </div>
                                &nbsp;
                                <a onClick={this.showFileTable}><i className={`fas fa-th view-icon ${this.state.listView ? "is-cadet-grey" : "is-white"}`}></i></a>
                                &nbsp;
                                <a onClick={this.showListView}><i className={`fas fa-th-list view-icon ${this.state.listView ? "is-white" : "is-cadet-grey"}`}></i></a>
                            </div>
                        </div>

                        <div className="file-box mg-top-10">
                            {this.state.loading ? <Loading /> :
                                this.state.listView ? this.renderListView() : this.renderFileTable()}
                        </div>
                    </section>

                </div>
            </div>
        )
    }
}
