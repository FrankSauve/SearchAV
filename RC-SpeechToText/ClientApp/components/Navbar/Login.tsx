import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { GoogleLogin } from 'react-google-login';
import { Redirect } from 'react-router-dom';
import axios from 'axios';
import auth from '../../Utils/auth';

var {GoogleClientId} = require("../../../appsettings.json");

interface State {
    redirectToVideos: boolean
    googleClientId: string
}

export default class Login extends React.Component<any, State> {

    constructor(props: RouteComponentProps<{}>) {
        super(props);

        this.state = {
            redirectToVideos: false,
            googleClientId: GoogleClientId
        }
    }

    public onLoginSuccess = (response:any) => {
        // Store Google JWT in localstorage
        auth.setAuthToken(response.tokenId);
        auth.setEmail(response.profileObj.email);
        auth.setProfilePicture(response.profileObj.imageUrl);
        auth.setName(response.profileObj.name);

        // User object
        const data = {
            name: response.profileObj.name,
            email: response.profileObj.email
        }
        
        const config = {
            headers: {
                'content-type': 'application/json'
            }
        }
        axios.post("/api/user/create", data, config)
            .then(user => {
                console.log("User created" + user);
            })
            .catch(err => alert(err));


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
                    className="fab fa-google button is-link"
                    clientId={this.state.googleClientId}
                    buttonText=" Me connecter avec Google"
                    onSuccess={this.onLoginSuccess}
                    onFailure={this.onLoginFailure}
                />
                {this.state.redirectToVideos ? <Redirect to="/dashboard"/> : null}
            </div>
        )
    }
}