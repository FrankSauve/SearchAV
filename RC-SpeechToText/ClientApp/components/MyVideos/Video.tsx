import * as React from 'react';

interface State {
    videoId: number,
    title: string,
    videoPath: string
    transcription: string,
    dateAdded: string
}

export default class Video extends React.Component<any, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            videoId: 0,
            title: this.props.title,
            videoPath: this.props.videoPath,
            transcription: this.props.transcription,
            dateAdded: this.props.dateAdded
        }
    }

    public render() {
        return (
            <div className="column is-one-quarter">
                <div className="card mg-top-30">
                     <header className="card-header">
                        <p className="card-header-title">
                        {this.state.title}
                        </p>
                    </header>
                    <div className="card-content">
                    <div className="content">
                        {this.state.transcription != null ? this.state.transcription.length > 100 ? this.state.transcription.substring(0,100) : this.state.transcription : null}
                    </div>
                            
                    </div>
                    <footer className="card-footer">
                        <p className="card-footer-item">
                            <span>
                                View
                            </span>
                        </p>
                        <p className="card-footer-item">
                            <span>
                                Edit
                            </span>
                        </p>
                    </footer>
                </div>
            </div>
        )
    }



}
