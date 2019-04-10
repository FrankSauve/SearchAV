import * as React from 'react';
import { Redirect } from 'react-router-dom';
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
    loggedUser: any;
    files: any[],
    allFiles: any[],
    versions: any[],
    currentFilterFiles: any[],
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
    allFilesSearch: boolean,
    unauthorized: boolean
}

export default class Dashboard extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            loggedUser: null,
            files: [],
            allFiles: [],
            versions:[],
            currentFilterFiles: [],
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
            allFilesSearch: false,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllFiles();
        this.getUserInfo();
    }

    public getUserInfo = () => {
        this.setState({ loading: true });

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/user/GetUserByEmail/' + auth.getEmail(), config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'loggedUser': res.data})
                this.setState({ 'loading': false });
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    // Logs out if the user's JWT is expired
                    auth.removeAuthToken();
                    this.setState({ 'unauthorized': true });
                }
            });
    }
    public getAllFilesLoaded = () => {
        this.deactivateFilters();

        var usernames = Array(this.state.allFiles.length);
        var counter = 0;

        this.state.allFiles.map((file) => {
                usernames[counter] = file.user.name;
                counter++;
        })

        this.setState({ 'files': this.state.allFiles });
        this.setState(
			{ 'currentFilterFiles': this.state.allFiles },
			this.searchDescription
		);
        this.setState({ 'usernames': usernames });

    }
    public getAllFiles = () => {
        //Keeping this method to update files once a new file is uploaded.
        this.setState({ loading: true });
        this.deactivateFilters();


        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/GetAllWithUsernameAndVersions', config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'files': res.data.files });
                this.setState({ 'allFiles': res.data.files });
                this.setState({ 'versions': res.data.versions})
				this.setState(
					{ 'currentFilterFiles': res.data.files },
					this.searchDescription
				);
                this.setState({ 'usernames': res.data.usernames })
                this.setState({ 'loading': false });
                console.log(this.state.loading);
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    // Logs out if the user's JWT is expired
                    auth.removeAuthToken();
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public getUserFiles = () => {
        this.deactivateFilters();

        var userFiles = Array(this.state.allFiles.length);
        var usernames = Array(this.state.allFiles.length);
        var counter = 0;

        this.state.allFiles.map((file) => {
            if (file.user.id == this.state.loggedUser.id) {
                userFiles[counter] = file;
                usernames[counter] = file.user.name;
                counter++;
            }
        })
		console.log(userFiles);
        this.setState({ 'files': userFiles.filter(n => n) });
        this.setState(
			{ 'currentFilterFiles': userFiles.filter(n => n) },
			this.searchDescription
			);
        this.setState({ 'usernames': usernames.filter(n => n) });
        this.setState({ 'isMyFilesFilterActive': true });
		console.log(this.state.files);

    }

    public getUserFilesToReview = () => {
        this.deactivateFilters();

        var userFiles = Array(this.state.allFiles.length);
        var usernames = Array(this.state.allFiles.length);
        var counter = 0;
        
        this.state.allFiles.map((file) => {
            if (file.reviewerId == this.state.loggedUser.id && file.flag.localeCompare("Revise", 'en', { sensitivity: 'base' }) != 0) {
                userFiles[counter] = file;
                usernames[counter] = file.user.name;
                counter++;
            }
        })

        this.setState({ 'files': userFiles.filter(n => n) });
		 this.setState(
			{ 'currentFilterFiles': userFiles.filter(n => n) },
			this.searchDescription
			);
        this.setState({ 'usernames': usernames.filter(n => n) });
        this.setState({ 'isFilesToReviewFilterActive': true });

    }

    public getAutomatedFiles = () => {
        this.deactivateFilters();

        var userFiles = Array(this.state.allFiles.length);
        var usernames = Array(this.state.allFiles.length);
        var counter = 0;

        this.state.allFiles.map((file) => {
            if (file.flag.localeCompare("Automatise", 'en', { sensitivity: 'base' }) == 0) {
                userFiles[counter] = file;
                usernames[counter] = file.user.name;
                counter++;
            }
        })

        this.setState({ 'files': userFiles.filter(n => n) });
		this.setState(
			{ 'currentFilterFiles': userFiles },
			this.searchDescription
			);
        this.setState({ 'usernames': usernames.filter(n => n) });

        this.setState({ 'isAutomatedFilterActive': true });

    }

    public getEditedFiles = () => {

        this.deactivateFilters();

        var userFiles = Array(this.state.allFiles.length);
        var usernames = Array(this.state.allFiles.length);
        var counter = 0;

        this.state.allFiles.map((file) => {
            if (file.flag.localeCompare("Edite", 'en', { sensitivity: 'base' }) == 0) {
                userFiles[counter] = file;
                usernames[counter] = file.user.name;
                counter++;
            }
        })

        this.setState({ 'files': userFiles.filter(n => n) });
		this.setState(
			{ 'currentFilterFiles': userFiles },
			this.searchDescription
			);
        this.setState({ 'usernames': usernames.filter(n => n) });

        this.setState({ 'isEditedFilterActive': true });

    }

    public getReviewedFiles = () => {
        this.deactivateFilters();

        var userFiles = Array(this.state.allFiles.length);
        var usernames = Array(this.state.allFiles.length);
        var counter = 0;

        this.state.allFiles.map((file) => {
            if (file.flag.localeCompare("Revise", 'en', { sensitivity: 'base' }) == 0) {
                userFiles[counter] = file;
                usernames[counter] = file.user.name;
                counter++;
            }
        })

        this.setState({ 'files': userFiles.filter(n => n) });
		this.setState(
			{ 'currentFilterFiles': userFiles },
			this.searchDescription
			);
        this.setState({ 'usernames': usernames.filter(n => n) });

        this.setState({ 'isReviewedFilterActive': true });

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
                                getAllFiles={this.getAllFilesLoaded}
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
                                getAllFiles={this.getAllFilesLoaded}
                            /> : <h1 className="title no-files">AUCUN FICHIERS</h1>}
            </div>
        )
    }

    private getCurrentVersion = (fileId: AAGUID) => {
        var currentVersion = this.state.versions[0];
        this.state.versions.map((version) => {
            console.log(version.fileId);
            if (version.fileId == fileId) {
                currentVersion = version;
            }
        })
        return currentVersion;
    }

    public searchDescription = () => {
        var searchTerms = this.state.searchTerms;
        var fileAddedTitle = false;
        var fileAddedDescription = false;
        var counter = 0;
        var results = Array(this.state.allFiles.length);
        var resultsUsernames = Array(this.state.allFiles.length);
        var currentVersion;

        console.log(this.state.versions);
        if (searchTerms == "") {
            this.setState({ 'files': this.state.currentFilterFiles });
        } else {
            this.state.currentFilterFiles.map((file) => {
                fileAddedTitle = false;
                currentVersion = this.getCurrentVersion(file.id);
                console.log(currentVersion);
                if (file.description != null) {
                    if (file.description.toLowerCase().includes(searchTerms.toLowerCase())) {
                        results[counter] = file;
						resultsUsernames[counter] = file.user.name;
						
                        //If file is added here we do not want to add it again if it has a title match too
                        fileAddedTitle = true;
                        counter++;
                    }
                }
                if (file.title != null && !fileAddedTitle) {
                    if (file.title.toLowerCase().includes(searchTerms.toLowerCase())) {
                        results[counter] = file;
						resultsUsernames[counter] = file.user.name;
                        counter++;
                        fileAddedDescription = true;
                    }
                }
                if (this.state.allFilesSearch && currentVersion != null && !fileAddedTitle && !fileAddedDescription) {
                    if (currentVersion.transcription.toLowerCase().includes(searchTerms.toLowerCase())) {
                        results[counter] = file;
                        resultsUsernames[counter] = file.user.name;
                        counter++;
                    }
                }
            })
            this.setState({ 'files': results.filter(n => n) });
            this.setState({ 'usernames': resultsUsernames.filter(n => n) });
        }
        
    }
    
    public handleAllFilesSearch = (e: any) => {
        this.setState(
            { 'allFilesSearch': e.target.checked },
            this.searchDescription
        );
    }
    public handleSearch = (e: any) => {
        this.setState(
            { 'searchTerms': e.target.value.trim() },
            this.searchDescription
        );
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
                {this.state.unauthorized ? <Redirect to="/"/> : null}
                <div className="columns">
                    <div className="column is-one-fifth">
                        <FileInput
                            getAllFiles={this.getAllFilesLoaded}
                        />

                        <a onClick={!this.state.isFilesToReviewFilterActive ? this.getUserFilesToReview : this.getAllFilesLoaded}>
                            <FilesToReviewFilter
                                isActive={this.state.isFilesToReviewFilterActive}
                            />
                        </a>
                        
                        <a onClick={!this.state.isAutomatedFilterActive ? this.getAutomatedFiles : this.getAllFilesLoaded}>
                            <AutomatedFilter
                                isActive={this.state.isAutomatedFilterActive}
                            />
                        </a>

                        <a onClick={!this.state.isEditedFilterActive ? this.getEditedFiles : this.getAllFilesLoaded}>
                            <EditedFilter
                                isActive={this.state.isEditedFilterActive}
                            />
                        </a>

                        <a onClick={!this.state.isReviewedFilterActive ? this.getReviewedFiles : this.getAllFilesLoaded}>
                            <ReviewedFilter
                                isActive={this.state.isReviewedFilterActive}
                            />
                        </a>

                        <a onClick={!this.state.isMyFilesFilterActive ? this.getUserFiles : this.getAllFilesLoaded}>
                            <MyFilesFilter
                                isActive={this.state.isMyFilesFilterActive}
                            />
                        </a>
                    </div>

                    <section className="section column tile-container">
                        <div className="search-div">
                            <div className="field is-horizontal mg-top-10">
                                <p className="is-cadet-grey search-title">{this.state.isMyFilesFilterActive ? "MES " : ""} FICHIERS {fileType}</p>
                                <div className="right-side">
                                    <div className="search-field">
                                        <input
                                            name="isGoing"
                                            type="checkbox"
                                            checked={this.state.allFilesSearch}
                                            onChange={this.handleAllFilesSearch} />
                                        <p className="control has-icons-right">
                                            <input className="input is-rounded search-input" type="search" onChange={this.handleSearch}/>
                                            <span className="icon is-small is-right">
                                                <i className="fas fa-search is-cadet-grey"></i>
                                            </span>
                                        </p>
                                    </div>
                                </div>
                                &nbsp;
                                <a onClick={this.showFileTable}><i className={`fas fa-th view-icon ${this.state.listView ? "is-cadet-grey" : "is-white"}`}></i></a>
                                &nbsp;
                                <a onClick={this.showListView}><i className={`fas fa-th-list view-icon ${this.state.listView ? "is-white" : "is-cadet-grey"}`}></i></a>
                            </div>
                        </div>

                        <div className="file-box">
                            {this.state.loading ? <Loading /> :
                                this.state.listView ? this.renderListView() : this.renderFileTable()}
                        </div>
                    </section>

                </div>
            </div>
        )
    }
}
