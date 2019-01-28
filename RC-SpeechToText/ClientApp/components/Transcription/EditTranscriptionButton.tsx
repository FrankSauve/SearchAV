import * as React from 'react';
import axios from "axios";
import auth from '../../Utils/auth';
import { Redirect } from 'react-router-dom';


export class EditTranscriptionButton extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public saveEditedTranscription = () => {
        const formData = new FormData();
        formData.append('versionId', this.props.versionId + '') //This converts versionId to a string 
        formData.append('oldTranscript', this.props.transcription)
        formData.append('newTranscript', this.props.editedTranscription)

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.post('/api/SaveEditedTranscript/SaveEditedTranscript', formData, config)
            .then(res => {
                console.log(res);
                this.props.updateVersionId(res.data.id);
                this.props.handleSubmit();
            })
            .catch(err => {
                if (err.response.status == 401) {
                    //this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {
        
        return (

        <div>

                <div>
                    {this.props.showEditButton ? (this.props.isEditing ? <a className="button is-danger" onClick={this.saveEditedTranscription}>Save</a> : <a className="button is-danger" onClick={this.props.editTranscription}>Edit</a>) : null} 
            </div>

        </div>
        );
    }
}
