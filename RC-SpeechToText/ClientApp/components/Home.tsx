import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface State {}

export class Home extends React.Component<RouteComponentProps<{}>, State> {
    public render() {
        return (
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
        );
    }
    
}
