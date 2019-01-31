import * as React from 'react';
import axios from 'axios';
import File from './File';
import { Redirect } from 'react-router-dom';
import auth from '../../Utils/auth';

interface State {
    files: any[],
    usernames: string[],
    loading: boolean,
    unauthorized: boolean
}

export default class FileTable extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: [],
            usernames: [],
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
        axios.get('/api/file/GetAllWithUsernames', config)
            .then(res => {
                console.log(res.data);
                this.setState({'files': res.data.value.files});
                this.setState({'usernames': res.data.value.usernames})
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

        var i = 0;

        return (
            <div className="has-text-centered">

                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}
                
                {this.state.loading ? progressBar : null}
                <div className="columns is-multiline">
                    {this.state.files.map((file) => {
                        const FileComponent = <File 
                                        fileId = {file.id}
                                        flag={file.flag}
                                        title = {file.title}
                                        username={this.state.usernames[i]}
                                        filePath = {file.filePath}
                                        dateAdded = {file.dateAdded}
                                        key = {file.fileId}
                                    />
                        i++; 
                        return(
                            FileComponent
                        )
                        
                    })}
                </div>
            </div>
        )
    }

    

}
