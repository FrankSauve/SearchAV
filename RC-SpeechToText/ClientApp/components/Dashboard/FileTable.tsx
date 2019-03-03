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
            files: this.props.files,
            usernames: this.props.usernames,
            loading: this.props.loading,
            unauthorized: false
        }
    }

    //Will update if props change
    public componentDidUpdate(prevProps : any) {
        if (this.props.files !== prevProps.files) {
            this.setState({ 'files': this.props.files });
        }

        if (this.props.usernames !== prevProps.usernames) {
            this.setState({ 'usernames': this.props.usernames });
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
                    {this.state.files.map((file) => {
                        const FileComponent = <File 
                                        fileId = {file.id}
                                        flag={file.flag}
                                        title = {file.title}
                                        description={file.description}
                                        username={this.state.usernames[i]}
                                        filePath = {file.filePath}
                                        dateAdded={file.dateAdded}
                                        number={file.notified}
                                        type={file.type}
                                        key = {file.id}
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
