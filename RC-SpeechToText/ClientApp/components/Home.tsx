import * as React from 'react';
import { RouteComponentProps, Redirect } from 'react-router';
import Login from './Navbar/Login';
import auth from '../Utils/auth';

interface State {}

export class Home extends React.Component<RouteComponentProps<{}>, State> {
    public render() {
        
        return (
            <div>
                {auth.isLoggedIn() ? <Redirect to="/dashboard"/> : null}
                <section className="hero is-medium is-danger is-bold">
                    <div className="hero-body">
                        <div className="container">
                            <h1 className="title">
                                STENO
                            </h1>
                            <h2 className="subtitle">
                                Transcription automatisé de fichiers audio et vidéos
                            </h2>
                        </div>
                    </div>
                </section>
                <div className="columns is-centered">
                    <div className="column is-one-quarter box has-text-centered">
                        <Login/>
                    </div>
                </div>
            </div>
        );
    }
    
}
