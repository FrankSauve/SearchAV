import * as React from 'react';
import { RouteComponentProps, Redirect } from 'react-router';
import Login from './Navbar/Login';
import auth from '../Utils/auth';

interface State { }

export class Home extends React.Component<RouteComponentProps<{}>, State> {
    public render() {

        return (
            <div>
                {auth.isLoggedIn() ? <Redirect to="/dashboard" /> : null}
                <section className="is-medium is-light">
                    <div>
                        <div className="container">
                            <h2 className="hometitle">
                                RBC
                                <a href="/">
                                    <img src="https://vignette.wikia.nocookie.net/logopedia/images/b/b7/Cbc-logo.png/revision/latest/scale-to-width-down/240?cb=20110304223128" width="30" height="30" />
                                </a>
                                Radio Canada
                            </h2>
                        </div>
                    </div>
                </section>
                <section className="hero is-small is-dark">
                    <div className="hero-body">
                        <div className="container">
                            <h2 className="homesubtitle">
                                CENTRE DES MEMBRES
                            </h2>
                        </div>
                    </div>
                </section>
                <div className="columns is-centered">
                    <p> L'application SGC Picto souhaite que vous vous identifiez </p>
                    <div className="column is-one-quarter has-text-centered mg-top-30">
                        <Login />
                    </div>
                </div>
            </div>
        );
    }

}
