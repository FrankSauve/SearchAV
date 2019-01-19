import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import { Redirect } from 'react-router-dom';

interface State {
    fileId: number,
    title: string,
    filePath: string, 
    transcriptionId: number,
    dateAdded: string, 
    type: string, 
    userId: number
}

export default class File extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            fileId: 0,
            title: this.props.title,
            filePath: this.props.filePath,
            transcriptionId: this.props.transcriptionId,
            dateAdded: this.props.dateAdded,
            type: this.props.type,
            userId: this.props.userId
        }
    }
    //TODO: for the transcription, get the right transcription from the transcription table with the TranscriptionId attribute in the File table.
    //{this.state.transcription != null ? this.state.transcription.length > 100 ? this.state.transcription.substring(0,100) : this.state.transcription : null}
    public render() {
        return (
            
            <div className="column is-one-quarter" >
                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}

                {this.state.editSuccess ? successMessage : null}
                <div className="card mg-top-30">
                     <header className="card-header">
                        <p className="card-header-title">
                        {this.state.title}
                        </p>
                    </header>
                    <div className="card-content">
                    <div className="content">
                      {this.state.transcriptionId}
                    </div>
                            
                    </div>
                    <footer className="card-footer">
                        <p className="card-footer-item">
                            <span>
                                View
                            </span>
                        </p>
                        <p className="card-footer-item">
                            <span>
                                Edit
                            </span>
                        </p>
                    </footer>
                </div>
            </div>

        )
    }



}
