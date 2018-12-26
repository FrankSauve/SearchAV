import * as React from 'react';


export class TranscriptionText extends React.Component<any> {
    constructor(props: any) {
        super(props);

    }

    public render() {
        
        return (
            <div>
                <h3 className="title is-3">{this.props.showText ? 'Automated transcript': ''}</h3>
                
                <p>
                    {(this.props.showText) ? this.props.text : ''}
                </p>
            </div>
        );
    }
}
