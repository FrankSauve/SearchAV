import * as React from 'react';
import { KeyboardEvent } from 'react';
import { ChangeEvent } from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import HighlightText from './HighlightText';

interface State {
    version: any,
    displayText: string,
    rawText: string,
    errorMessage: string
    words: any
}

export class TranscriptionText extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            version: this.props.version,
            displayText: this.rawToHtml(this.props.version.transcription),
            rawText: this.props.version.transcription,
            errorMessage: "",
            words: null
        }
    }

    // Called when the component renders
    componentDidMount() {
        this.getWordsByVersionId();
    }

    public getWordsByVersionId = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/word/GetByVersionId/' + this.props.version.id, config)
            .then(res => {
                this.setState({ words: res.data });
            })
            .catch(err => console.log(err));
    }

    rawToHtml(text: string) {
        return text.replace(/<br\s*[\/]?>/gi, "\n");
    }

    public handleBlur = () => {
        console.log('Returning:', this.state.rawText);
        this.props.handleChange(this.state.rawText)
    }

    public handleChange = (e: ChangeEvent<HTMLTextAreaElement>) => {
        var a = e.target.value.replace(/(?:\r\n|\r|\n)/g, '<br>')
        this.setState({ rawText: a });
        this.setState({ displayText: e.target.value })
    }

    public render() {
        return (
            <div>
                <textarea
                    className="textarea mg-top-30"
                    rows={20} //Would be nice to adapt this to the number of lines in the future
                    onChange={this.handleChange}
                    value={this.state.displayText}
                    onBlur={this.handleBlur}
                />
                {this.state.words ? <HighlightText version={this.props.version} words={this.state.words} /> : null}
            </div>
        );
    }
}
