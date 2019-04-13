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
        this.setState({ 'title': newTitle.split(".")[0] });
    }

    public updateDescription = (newDescription: string) => {
        this.setState({ 'description': newDescription });
    }

    removeTags(text: string) {
        let a = text;
        a = a.replace(/<span[^>]+\>/g, '');
        a = a.replace(/<\/span>/g, '');
        a = a.replace(/<a[^>]+\>/g, '');
        a = a.replace(/<\/a>/g, '');
        a = a.replace(/<div\s*[\/]?>/gi, " ");
        a = a.replace(/<br\s*[\/]?>/gi, " ");
        return a;
    }

    public formatTime = (dateModified: any) => {
        var d = new Date(dateModified);

        var day = d.getDate() < 10 ? "0" + d.getDate() : d.getDate();
        var month = d.getMonth() < 10 ? "0" + (d.getMonth() + 1) : (d.getMonth() + 1);
        var hours = d.getHours() < 10 ? "0" + d.getHours() : d.getHours();
        var minutes = d.getMinutes() < 10 ? "0" + d.getMinutes() : d.getMinutes();

        var datestring = day + "-" + month + "-" + d.getFullYear() + " " + hours + ":" + minutes;

        return datestring;

    }

    public render() {
        return (
            <div className="column is-3">
                <div className="card fileCard">
                    <Link className="info" to={`/FileView/${this.props.file.id}`}>
                        <span className={`tag is-rounded flag ${this.state.flag.indexOf("A") == 0 ? "is-danger" : this.state.flag.indexOf("R") == 0 ? "is-success has-text-black" : "is-info has-text-black"}`}><b>{this.state.flag.toUpperCase()}</b></span> 
                    </Link>
                    <header className="card-header">
                        <p className="card-header-title fileTitle">
                            <Link className="info" to={`/FileView/${this.props.file.id}`}>
                                {this.state.title ? (this.state.title.length < 35 ? this.state.title : this.state.title.substring(0, 35) + " ...") : null}
                            </Link>
                        </p>

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

                    <Link className="info" to={`/FileView/${this.props.file.id}`}>
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
                                <p>{this.state.description ? this.removeTags(this.state.description) : this.removeTags(this.props.transcription)}</p>
                            </div>
                            <br />
                            <p className="font-size-12"><b>{this.props.username}</b></p>
                            <time className="font-size-12" dateTime={this.props.date}>{this.formatTime(this.props.date)}</time>
                        </div>
                    </div>
                    </Link>
                </div>
            </div>
        );
    }
}