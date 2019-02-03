import * as React from 'react';


export class DescriptionText extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div>
                <textarea
                    className="textarea"
                    defaultValue={this.props.text}
                />
            </div>
        );
    }
}
