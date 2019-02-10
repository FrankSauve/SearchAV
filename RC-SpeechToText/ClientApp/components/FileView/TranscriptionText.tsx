import * as React from 'react';


export class TranscriptionText extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div>
                <textarea
                    className="textarea mg-top-30"
                    rows={20} //Would be nice to adapt this to the number of lines in the future
                    defaultValue={this.props.text}
                    onChange={this.props.handleChange}
                />
            </div>
        );
    }
}
