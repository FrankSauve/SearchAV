import * as React from 'react';


export class EditTranscriptionButton extends React.Component<any> {
    constructor(props: any) {
        super(props);

    }

    public render() {
        
        return (
            <div>
                {this.props.showEditButton ? (this.props.isEditing ? <a className="button is-danger" onClick={this.props.handleSubmit}>Save</a> : <a className="button is-danger" onClick={this.props.editTranscription}>Edit</a>) : null} 
            </div>
        );
    }
}
