import * as React from 'react';
import axios from 'axios';
import Version from './Version';
import { Redirect } from 'react-router-dom';
import auth from '../../Utils/auth';

interface State {
    versions: any[],
    usernames: string[],
    dates: string[],
    loading: boolean,
    unauthorized: boolean
}

export default class VersionTable extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            versions: this.props.versions,
            usernames: this.props.usernames,
            dates: this.props.dates,
            loading: this.props.loading,
            unauthorized: false
        }
    }

    //Will update if props change
    public componentDidUpdate(prevProps : any) {
        if (this.props.versions !== prevProps.versions) {
            this.setState({ 'versions': this.props.versions });
        }

        if (this.props.usernames !== prevProps.usernames) {
            this.setState({ 'usernames': this.props.usernames });
        }

        if (this.props.dates !== prevProps.dates) {
            this.setState({ 'dates': this.props.dates });
        }
    }

    public render() {

        const progressBar = <img src="assets/loading.gif" alt="Loading..."/>

        var i = 0;

        return (
            <div>

                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}
                
                {this.state.loading ? progressBar : null}
                <div className="columns is-multiline">
                    {this.state.versions.map((version) => {
                        const VersionComponent = <Version
                                        version={this.state.versions}
                                        username={this.state.usernames[i]}
                                        dates={this.state.dates}
                                    />
                        i++; 
                        return(
                            VersionComponent
                        )
                        
                    })}
                </div>
            </div>
        )
    }

    

}
