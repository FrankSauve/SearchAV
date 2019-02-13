import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import Login from './Login';
import Logout from './Logout';
import { SelectReviewerModal } from '../Modals/SelectReviewerModal';
import auth from '../../Utils/auth'

interface State {
    showModal: boolean
}

export default class Navbar extends React.Component<RouteComponentProps<{}>, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            showModal: false
        }
    }

    public showModal = () => {
        this.setState({ showModal: true });
    }

    public hideModal = () => {
        this.setState({ showModal: false });
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
                        <img className="is-circular" src={auth.getProfilePicture()!} />
                    </div>
                    <div className="navbar-item">
                        <Logout />
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
                    <a className="button is-rounded mg-top-10 mg-left-400" onClick={this.showModal}><i className="far fa-envelope"></i> Demander une revision</a>
                    <a className="button is-rounded mg-top-10 mg-left-10"><i className="fas fa-file-export"></i> Exporter</a>
                <a className="button is-rounded is-link mg-top-10 mg-left-10"><i className="far fa-save"></i> Enregistrer</a>
                </div>
            </div>
        );
    }

    public renderNormalNav = () => {
        return(
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
                <nav className="navbar is-light" role="navigation" aria-label="main navigation">
                    <div className="navbar-brand">
                        <a className="navbar-item" href="/">
                            <img src="https://vignette.wikia.nocookie.net/logopedia/images/b/b7/Cbc-logo.png/revision/latest/scale-to-width-down/240?cb=20110304223128" width="30" height="30" />
                        </a>
                        
                        {this.isInFileViewPage() ? this.renderFileViewNav() : this.renderNormalNav()}

                        <SelectReviewerModal
                            showModal={this.state.showModal}
                            hideModal={this.hideModal}
                        />
                        

                            <a role="button" className="navbar-burger burger" aria-label="menu" aria-expanded="false" data-target="navbarBasicExample">
                                <span aria-hidden="true"></span>
                                <span aria-hidden="true"></span>
                                <span aria-hidden="true"></span>
                            </a>
                        </div>

                        {auth.isLoggedIn() ? this.renderLoggedIn(): null }

                </nav>
            </div>
                )
            }
        }
