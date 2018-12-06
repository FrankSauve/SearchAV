import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import Login from './Login';
import Logout from './Logout';
import auth from '../../Utils/auth'

interface State {

}

export default class Navbar extends React.Component<RouteComponentProps<{}>, State> {
    public render() {
        return (
            <div>
                <nav className="navbar is-dark" role="navigation" aria-label="main navigation">
                    <div className="navbar-brand">
                        <a className="navbar-item" href="/">
                            <img src="https://vignette.wikia.nocookie.net/logopedia/images/b/b7/Cbc-logo.png/revision/latest/scale-to-width-down/240?cb=20110304223128" width="30" height="30" />
                        </a>

                        <a role="button" className="navbar-burger burger" aria-label="menu" aria-expanded="false" data-target="navbarBasicExample">
                            <span aria-hidden="true"></span>
                            <span aria-hidden="true"></span>
                            <span aria-hidden="true"></span>
                        </a>
                    </div>

                    <div id="navbarBasicExample" className="navbar-menu">
                        <div className="navbar-start">
                            <Link className="navbar-item" to="/">
                                Accueil
                            </Link>

                            <Link className="navbar-item" to="/TestAccuracy">
                                Test Google's Accuracy
                            </Link>

                            <Link className="navbar-item" to="/Transcription">
                                Transcrire vidéo
                            </Link>

                            <Link className="navbar-item" to="/videos">
                                Mes vidéos
                            </Link>
                        </div>

                        <div className="navbar-end">
                            <div className="navbar-item">
                                {auth.isLoggedIn() ? <Logout /> : <Login />}
                            </div>
                        </div>
                    </div>
                </nav>
            </div>
        )
    }
}
