import * as React from 'react';


interface State {
    title: string
}

export class FileCard extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = { title: this.props.title }
    }

    public render() {
        return (
            <div className="column is-3">
                <div className="card mg-top-30 fileCard">
                    <header className="card-header">
                        {this.props.flag != null ? <span className="tag is-danger">{this.props.flag}</span> : null}
                        <p className="card-header-title fileTitle">
                            {this.state.title.substring(0, this.state.title.lastIndexOf('.'))}
                        </p>
                   
                </header>
                    <div className="card-image">
                        <div className="hovereffect">
                    <figure className="image is-4by3">
                                <img src={this.props.image} alt="Placeholder image" />
                                <div className="overlay">
                                    <a className="info" href="/FileView">View/Edit</a>
                                </div>
                            </figure>
                        </div>
                </div>
                    <div className="card-content">

                        <div className="content fileContent">
                            <p className="transcription">{this.props.transcription}</p>
                            <p><b>{this.props.username}</b></p>
                            <time dateTime={this.props.date}>{this.props.date}</time>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
