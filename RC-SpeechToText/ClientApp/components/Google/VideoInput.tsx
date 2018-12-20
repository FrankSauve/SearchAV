import * as React from 'react';


export class VideoInput extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        
        return (
            <div>
                <div className="file has-name">
                    <label className="file-label">
                        <input className="file-input" type="file" name="resume" onChange={this.props.onChange}/>
                        <span className="file-cta">
                            <span className="file-icon">
                                <i className="fas fa-upload"></i>
                            </span>
                            <span className="file-label">
                                Fichier Audio…
                            </span>
                        </span>
                        <span className="file-name">
                              {(this.props.audioFile == null) || (this.props.audioFile == undefined) ? null : this.props.audioFile.name}
                        </span>
                    </label>
                </div>
            </div>
        );
    }
}
