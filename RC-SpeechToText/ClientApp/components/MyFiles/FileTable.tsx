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

export default class FileTable extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            files: this.props.files,
            loading: this.props.loading,
            unauthorized: this.props.unauthorized
        }
    }

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
                                fileId = {file.id}
                                title={file.title}
                                flag={file.flag}
                                userId={file.userId}
                                filePath = {file.filePath}
                                transcription = {file.transcription}
                                dateAdded = {file.dateAdded}
                                key = {file.id}
                            />
                        )
                    })}
                </div>
            </div>
        )
    }

    

}
