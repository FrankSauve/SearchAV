import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { GoogleLogin } from 'react-google-login';
import { Redirect } from 'react-router-dom';
import { AppConfiguration } from "read-appsettings-json";
import auth from '../../Utils/auth';

interface State {
    redirectToVideos: boolean
}

export default class Login extends React.Component<any, State> {

    constructor(props: RouteComponentProps<{}>) {
        super(props);

        this.state = {
            redirectToVideos: false
        }
    }

    public onLoginSuccess = (response:any) => {
        console.log("Google login response: " + response);
        auth.setAuthToken(response.tokenId);
        this.setState({'redirectToVideos': true})
    };

    public onLoginFailure = (response: any) => {
        console.log("Google login response: " + response);
        alert("Failed login");
    };

    public render() {
        return (
            <div>
                <GoogleLogin
                    className="fab fa-google button is-danger"
                    clientId={AppConfiguration.Setting().GoogleClientId}
                    buttonText=" Login avec Google"
                    onSuccess={this.onLoginSuccess}
                    onFailure={this.onLoginFailure}
                />
                {this.state.redirectToVideos ? <Redirect to="/files"/> : null}
            </div>
        )
    }
}