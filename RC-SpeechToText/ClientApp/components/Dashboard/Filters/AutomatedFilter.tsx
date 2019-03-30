import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

interface State {
    files: any[],
    unauthorized: boolean,
    loading: boolean
}

export default class AutomatedFilter extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            unauthorized: false,
            loading: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAutomatedFiles();
    }

    public getAutomatedFiles = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllFilesByFlag/Automatise', config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data.files });
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {   
        return (
            <div className={`card filters mg-top-5 ${this.props.isActive ? "has-background-blizzard-blue" : "has-background-link"}`}>
                <div className="card-content">
                    <p className={`title ${this.props.isActive ? "is-link" : "automated" }`}>
                        {this.state.files.length}
                    </p>
                    <p className={`subtitle ${this.props.isActive ? "is-link" : "automated"}`}>
                        <b>FICHIERS<br />
                            TRANSCRITS</b>
                </p>
                </div>
            </div>
        )
    }
}