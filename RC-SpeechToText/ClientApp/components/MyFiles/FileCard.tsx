import * as React from 'react';


export class FileCard extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div className="column is-one-quarter" >
                <div className="card mg-top-30">
                    <header className="card-header">
                        {this.props.flag != null ? <span className="tag is-danger">{this.props.flag}</span> : null}
                        <p className="card-header-title">
                            {this.props.title}
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

                        <div className="content">
                            {this.props.transcription}
                            <time dateTime={this.props.date}>{this.props.date}</time>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
