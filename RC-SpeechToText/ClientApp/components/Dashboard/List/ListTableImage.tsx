import * as React from 'react';
import axios from 'axios';
import auth from '../../../Utils/auth';

interface State {
    transcription: string,
    unauthorized: boolean
}

export default class ListTableImage extends React.Component<any, State>
{
    constructor(props: any) {
        super(props);
        this.state = {
            transcription: "",
            unauthorized: false

        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getActiveTranscription();
    }

    public getActiveTranscription = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/version/GetActivebyFileId/' + this.props.fileId, config)
            .then(res => {
                console.log(res);
                this.setState({ 'transcription': res.data.transcription })
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    rawToWhiteSpace(text: string) {
        return text.replace(/<br\s*[\/]?>/gi, " ");
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
                            <strong>{this.props.title}</strong> <small className={`tag is-rounded flag ${this.props.flag.indexOf("A") == 0 ? "is-danger" : this.props.flag.indexOf("R") == 0 ? "is-success has-text-black" : "is-info has-text-black"}`}>{this.props.flag.toUpperCase()}</small>
                            <br />
                            <br />
                            <p className="transcription">{this.props.description ? this.rawToWhiteSpace(this.props.description) : this.state.transcription.length > 100 ? this.rawToWhiteSpace(this.state.transcription.substring(0, 100)) + " ..." : this.rawToWhiteSpace(this.state.transcription)}</p>
                        </p>
                    </div>
                </article>
            </div>
        )
    }
}