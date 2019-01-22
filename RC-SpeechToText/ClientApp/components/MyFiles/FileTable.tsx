import * as React from 'react';
import axios from 'axios';
import File from './File';
import { RouteComponentProps } from 'react-router';
import { Redirect } from 'react-router-dom';
import auth from '../../Utils/auth';

interface State {
    files: any[]
    loading: boolean,
    unauthorized: boolean
}

export default class FileTable extends React.Component<RouteComponentProps<{}>, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            loading: true,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllFiles();
    }

    public getAllFiles = () => {
        this.setState({'loading': true});
        
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/file/index', config)
            .then(res => {
                this.setState({'files': res.data});
                this.setState({'loading': false});
            })
            .catch(err => {
                if(err.response.status == 401) {
                    this.setState({'unauthorized': true});
                }
            });
    };

    public render() {

        const progressBar = <img src="assets/loading.gif" alt="Loading..."/>

        return (
            <div className="container has-text-centered">

                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}
                
                {this.state.loading ? progressBar : null}
                <div className="columns is-multiline">
                    {this.state.files.map((file) => {
                        return (
                            <File 
                                fileId = {file.fileId}
                                title = {file.title}
                                filePath = {file.filePath}
                                transcription = {file.transcription}
                                dateAdded = {file.dateAdded}
                                key = {file.fileId}
                            />
                        )
                    })}
                </div>
            </div>
        )
    }

    

}
