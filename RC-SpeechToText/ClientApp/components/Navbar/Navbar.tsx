import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import Login from './Login';
import Logout from './Logout';
import { SelectReviewerModal } from '../Modals/SelectReviewerModal';
import auth from '../../Utils/auth'
import { ExportModal } from '../Modals/ExportModal';

interface State {
    showReviewModal: boolean,
    showExportModal: boolean
    showDropdown: boolean
}

export default class Navbar extends React.Component<RouteComponentProps<{}>, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            showReviewModal: false,
            showExportModal: false,
            showDropdown: false
        }
    }

    // Add event listener for a click anywhere in the page
    componentDidMount() {
        document.addEventListener('mouseup', this.hideDropdown);
    }
    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.hideDropdown);
    }

    public showReviewModal = () => {
        this.setState({ showReviewModal: true });
    }

    public hideModal = () => {
        this.setState({ showReviewModal: false });
    }

    public showExportModal = () => {
        this.setState({ showExportModal: true });
    }

    public hideExportModal = () => {
        this.setState({ showExportModal: false });
    }

    public showDropdown = () => {
        this.setState({ showDropdown: true });
    }

    public hideDropdown = () => {
        this.setState({ showDropdown: false });
    }


    // Shown when the user is logged in
    public renderLoggedIn = () => {
        return (
            <div className="navbar-menu">
                <div className="navbar-end">
                    <div className="navbar-item">
                        <p>{auth.getName()!}</p>
                    </div>
                    <div className="navbar-item">
                        <div>
                            <div className={`dropdown is-right ${this.state.showDropdown ? "is-active" : null}`} >
                                <div className="dropdown-trigger">
                                    <div aria-haspopup="true" aria-controls="dropdown-menu4" onClick={this.showDropdown}>
                                        <img className="is-circular" src={auth.getProfilePicture()!} />
                                    </div>
                                </div>
                                <div className="dropdown-menu" id="dropdown-menu4" role="menu">
                                    <div className="dropdown-content">
                                        <Logout />
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>
        );
    }
        
    public isInFileViewPage() {
        var re = /^.*(FileView).*$/; // Letters only regex
                return re.test(window.location.pathname);
            }
        
    public renderFileViewNav = () => {
        return (
            <div className="navbar-menu">
                    <div className="navbar-start">
                        <Link className="navbar-item" to="/dashboard">
                            <i className="fas fa-angle-left"></i>
                        </Link>
                        <a className="button is-rounded mg-top-10 mg-left-400" onClick={this.showReviewModal}><i className="far fa-envelope mg-right-5"></i> Demander une revision</a>
                        <a className="button is-rounded mg-top-10 mg-left-10" onClick={this.showExportModal}><i className="fas fa-file-export mg-right-5"></i>Exporter</a>
                        <a className="button is-rounded is-link mg-top-10 mg-left-10"><i className="far fa-save mg-right-5"></i> Enregistrer</a>
                    </div>

                    <ExportModal
                        showModal={this.state.showExportModal}
                        hideModal={this.hideExportModal}
                        title={"Exporter un fichier"}
                        onConfirm={this.hideExportModal}
                    />

                </div>



                );
            }
        
    public renderNormalNav = () => {
        return (
            <div className="navbar-menu">
                    <div className="navbar-start">
                        <Link className="navbar-item" to="/">
                            STENO
                    </Link>
                    </div>
                </div>
                );
            }
        
    public render() {
        
        return (
            <div>
                <nav className="navbar is-light container" role="navigation" aria-label="main navigation">
                        <div className="navbar-brand">
                            <a className="navbar-item" href="/">
                                <img src="https://vignette.wikia.nocookie.net/logopedia/images/b/b7/Cbc-logo.png/revision/latest/scale-to-width-down/240?cb=20110304223128" width="30" height="30" />
                            </a>

                            {this.isInFileViewPage() ? this.renderFileViewNav() : this.renderNormalNav()}

                            <SelectReviewerModal
                                showModal={this.state.showReviewModal}
                                hideModal={this.hideModal}
                            />


                            <a role="button" className="navbar-burger burger" aria-label="menu" aria-expanded="false" data-target="navbarBasicExample">
                                <span aria-hidden="true"></span>
                                <span aria-hidden="true"></span>
                                <span aria-hidden="true"></span>
                            </a>
                        </div>

                        {auth.isLoggedIn() ? this.renderLoggedIn() : null}

                    </nav>
                </div>
                )
            }
        }
