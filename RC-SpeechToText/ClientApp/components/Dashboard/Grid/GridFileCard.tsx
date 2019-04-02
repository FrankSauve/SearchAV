import * as React from 'react';
import { Link } from 'react-router-dom';
import { DropdownButton} from '../DropdownButton';

interface State {
    title: string,
    description: string,
    duration: string, 
    flag: string,
}

export class GridFileCard extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            title: this.props.title,
            description: this.props.description,
            duration: this.props.duration, 
            flag: this.props.flag,
        }
    }

    public updateTitle = (newTitle: string) => {
        this.setState({ 'title': newTitle });
    }

    public updateDescription = (newDescription: string) => {
        this.setState({ 'description': newDescription });
    }

    rawToWhiteSpace(text: string) {
        return text.replace(/<br\s*[\/]?>/gi, " ");
    }

    removeExtension(title: string) {
        var titleNoExtension = title.lastIndexOf('.') != -1 ? title.substring(0, title.lastIndexOf('.')) : title;
        return titleNoExtension;
    }

    public render() {
        return (
            <div className="column is-3">
                <div className="card fileCard">
                    <span className={`tag is-rounded flag ${this.state.flag.indexOf("A") == 0 ? "is-danger" : this.state.flag.indexOf("R") == 0 ? "is-success has-text-black" : "is-info has-text-black"}`}>{this.state.flag.toUpperCase()}</span> 
                    <header className="card-header">
                        <p className="card-header-title fileTitle">
                            {this.state.title ? (this.state.title.length < 35 ? this.removeExtension(this.state.title) : this.removeExtension(this.state.title).substring(0, 35) + " ..." ) : null}</p>
                        
                         <DropdownButton
                            fileId={this.props.file.id}
                            title={this.props.file.title}
                            description={this.props.file.description}
                            flag={this.props.file.flag}
                            updateFiles={this.props.updateFiles}
                            username={this.props.username}
                            image={this.props.file.type == "Audio" ? 'assets/speakerIcon.png' : this.props.file.thumbnailPath}
                            date={this.props.file.dateAdded.substring(0, 10) + " " + this.props.file.dateAdded.substring(11, 16)}
                            updateTitle={this.updateTitle}
                            updateDescription={this.updateDescription}
                          />

                    </header>
                    <div className="card-image">
                        <div className="hovereffect">
                            <figure className="image is-16by9">
                                <img src={this.props.image} alt="Placeholder image" />
                                <div className="time-on-thumbnail-rectangle">
                                    <p className="time-on-thumbnail-police">{this.state.duration}</p>
                                </div>
                                <div className="overlay">
                                    <Link className="info" to={`/FileView/${this.props.file.id}`}>Voir/Modifier</Link>
                                </div>                                
                            </figure>
                        </div>
                    </div>
                    <div className="card-content">
                        <div className="content fileContent">
                            <div className="transcription-grid-view">
                                <p>{this.state.description ? this.rawToWhiteSpace(this.state.description) : this.rawToWhiteSpace(this.props.transcription)}</p>
                            </div>
                            <br />
                            <p className="font-size-12 font-family-roboto"><b>{this.props.username}</b></p>
                            <time className="font-size-12 font-family-roboto" dateTime={this.props.date}>{this.props.date}</time>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}