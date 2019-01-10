import * as React from 'react';


export class TranscriptionText extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div>
                <h3 className="title is-3">{this.props.showText ? 'Automated transcript': ''}</h3>

                {this.props.isEditing ? <textarea
                        className="textarea"
                        onChange={this.props.handleEditChange}
                        rows={6} //Would be nice to adapt this to the number of lines in the future
                        defaultValue={this.props.text}
                        /> : <p>{(this.props.showText) ? this.props.text : ''}</p>}
            </div>
        );
    }
}
