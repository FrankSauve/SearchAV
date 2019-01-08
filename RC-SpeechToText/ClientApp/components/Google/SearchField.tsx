import * as React from 'react';


export class SearchField extends React.Component<any> {
    constructor(props: any) {
        super(props);

    }

    public render() {
        
        return (
            <div>
                {(this.props.showText) ? 
                    <div>
                        <div className="mg-top-30 field is-horizontal">
                            <a className="button is-danger mg-right-30" onClick={this.props.searchTranscript}> Recherche </a>
                            <input className="input" type="text" placeholder="Your search terms" onChange={this.props.updateSearchTerms} />
                        </div>

                        {this.props.timestamps == '' ? null : <p> Résultats : {this.props.timestamps} </p>}
                    </div>
                    : ''}
            </div>
        );
    }
}
