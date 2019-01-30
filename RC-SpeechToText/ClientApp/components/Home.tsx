import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import Login from './Navbar/Login';

interface State {}

export class Home extends React.Component<RouteComponentProps<{}>, State> {
    public render() {
        return (
            <div>
                <section className="hero is-medium is-danger is-bold">
                    <div className="hero-body">
                        <div className="container">
                            <h1 className="title">
                                SearchAV
                            </h1>
                            <h2 className="subtitle">
                                Un projet capstone de Concordia
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
