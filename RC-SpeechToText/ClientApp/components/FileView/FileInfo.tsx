import * as React from 'react';

interface State {
    test: string,
}

export class FileInfo extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            test: this.props.test 
        }
    }


    public render() {
        return (
            <div className = "columns">
                <div className="column">
                    {<b>Image associ&#233;e: </b>}
                    {<p> Placeholder Image </p>}

                </div>
                <div className="column">
                    {<b>Date de modification: </b>}
                    {<p> Placeholder Date </p>}
                    {<b>Import&#233; par: </b>}
                    {<p> Placeholder Name </p>}

                </div>

            </div>
        );
    }
}