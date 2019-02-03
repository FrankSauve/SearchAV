import * as React from 'react';


export class TranscriptionText extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div>
                <textarea
                    className="textarea"
                    rows={20} //Would be nice to adapt this to the number of lines in the future
                    defaultValue={this.props.text}
                />
            </div>
        );
    }
}
