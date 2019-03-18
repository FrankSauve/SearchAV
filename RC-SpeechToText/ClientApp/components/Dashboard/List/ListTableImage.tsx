﻿import * as React from 'react';

export default class ListTableImage extends React.Component<any>
{
    constructor(props: any) {
        super(props);

    }

    public render() {
        return (
            <div>
                <article className='media'>
                    <figure className="media-left">
                        <p className='image is-96x96'>
                            <a href={`/FileView/${this.props.fileId}`}><img src={this.props.thumbnailPath} alt="Placeholder image"></img></a>

                        </p>
                    </figure>
                    <div className="media-content">
                        <p>
                            <strong>{this.props.title}</strong> <small className={`tag is-rounded ${this.props.flag.indexOf("A") == 0 ? "is-danger" : this.props.flag.indexOf("R") == 0 ? "is-success" : "is-info"}`}>{this.props.flag}</small>
                            <br />
                            <br />
                            {this.props.description}
                        </p>
                    </div>
                </article>
            </div>
        )
    }
}