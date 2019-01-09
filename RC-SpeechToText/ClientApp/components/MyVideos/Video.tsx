import * as React from 'react';
import axios from 'axios';

interface State {
    videoId: number,
    title: string,
    videoPath: string
    transcription: string,
    dateAdded: string,
    isEditing: boolean,
    editedTranscription: string
}

export default class Video extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            videoId: this.props.videoId,
            title: this.props.title,
            videoPath: this.props.videoPath,
            transcription: this.props.transcription,
            dateAdded: this.props.dateAdded,
            isEditing: false,
            editedTranscription: ""
        }
    }

    public saveTranscription = () => {
        const formData = new FormData();
        formData.append('videoId', this.state.videoId + '') //This converts videoId to a string
        formData.append('oldTranscript', this.state.transcription)
        formData.append('newTranscript', this.state.editedTranscription)

        const config = {
            headers: {
                'content-type': 'application/json'
            }
        }

        axios.post('/api/SavingTranscript/SaveTranscript', formData, config)
            .then(res => {
                console.log(res);
                this.handleSubmit();
            })
            .catch(err => console.log(err));
    }

    public editTranscription = () => {
        this.setState({ 'isEditing': true })
    }

    public handleChange = (e: any) => {
        this.setState({ editedTranscription: e.target.value })
    }

    public handleSubmit = () => {
        this.setState({ transcription: this.state.editedTranscription })
        this.setState({ 'isEditing': false })
    }

    public handleCancel = () => {
        this.setState({ 'isEditing': false })
    }

    public render() {   
        const isEditingContent = <div className="card-content">
            <div className="content">
                <textarea
                    className="textarea"
                    onChange={this.handleChange}
                    rows={6}
                    defaultValue={this.state.transcription}
                /> 
            </div>
        </div>

        const notEditingContent = <div className="card-content">
            <div className="content">
                {this.state.transcription != null ? this.state.transcription.length > 100 ? this.state.transcription.substring(0, 100) : this.state.transcription : null}
            </div>

        </div>

        const isEditingFooter = <footer className="card-footer">
                                    <a onClick={this.handleCancel} className="card-footer-item">Cancel</a>
                                    <a onClick={this.saveTranscription} className="card-footer-item">Save</a>
                                </footer>

        const notEditingFooter = <footer className="card-footer">
                                    <a className="card-footer-item">View</a>
                                    <a onClick={this.editTranscription} className="card-footer-item">Edit</a>
                                 </footer>

        return (
            <div className="column is-one-quarter">
                <div className="card mg-top-30">
                     <header className="card-header">
                        <p className="card-header-title">
                        {this.state.title}
                        </p>
                    </header>
                    {this.state.isEditing ? isEditingContent : notEditingContent}
                    {this.state.isEditing ? isEditingFooter : notEditingFooter}
                </div>
            </div>
        )
    }



}
