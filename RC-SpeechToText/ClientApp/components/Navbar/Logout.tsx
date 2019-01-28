import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { GoogleLogout } from 'react-google-login';
import { Redirect } from 'react-router-dom';
import auth from '../../Utils/auth';

interface State {
    redirectToHome: boolean
}

export default class Logout extends React.Component<any, State> {

    constructor(props: RouteComponentProps<{}>) {
        super(props);

        this.state = {
            redirectToHome: false
        }
    }

    public onLogout = () => {
        auth.removeAuthToken();
        this.setState({'redirectToHome': true})
    };

    public render() {
        return (
            <div>
                <button className="button is-light" onClick={this.onLogout}>Logout</button>
                {this.state.redirectToHome ? <Redirect to="/"/> : null}
            </div>
            
        )
    }
}