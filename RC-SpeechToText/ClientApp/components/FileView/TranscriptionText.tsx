import * as React from 'react';
import { ChangeEvent } from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    version: any,
    displayText: string,
    rawText: string,
    errorMessage: string
    firstSelectedWord: string,
    lastSelectedWord: string
}

export class TranscriptionText extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            version: this.props.version,
            displayText: this.rawToHtml(this.props.version.transcription),
            rawText: this.props.version.transcription,
            errorMessage: "",
            firstSelectedWord: '',
            lastSelectedWord: ''
        }
    }

    // Called when the component renders
    componentDidMount() {
        // Add event listener for a click anywhere in the page
        document.addEventListener('mouseup', this.getHighlightedWords);
    }

    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.getHighlightedWords);
    }

    public getHighlightedWords = (event: any) => {
        let s = document.getSelection();
        let selectedWords = s ? s.toString().split(" ") : null;
        if (selectedWords) {
            //Get first non-empty string element
            for (var i = 0; i < selectedWords.length; i++) {
                if (selectedWords[i].localeCompare("") != 0) {
                    this.setState({ firstSelectedWord: selectedWords[i] });
                    break;
                }
            }
            //empty string means no selection, therefore only get second word and call the search func if there is at least one word selected
            if (this.state.firstSelectedWord.localeCompare("") != 0) {
                //get last non-empty string element
                for (var i = selectedWords.length - 1; i >= 0; i--) {
                    if (selectedWords[i].localeCompare("") != 0) {
                        this.setState({ lastSelectedWord: selectedWords[i] });
                        break;
                    }
                }

                console.log("firstSelectedWord: " + this.state.firstSelectedWord + ", lastSelectedWord: " + this.state.lastSelectedWord);
                this.searchTranscript();
            }
        }
    }

    public searchTranscript = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        console.log(this.props.version.id);
        axios.get('/api/Transcription/SearchTranscript/' + this.state.version.id + '/' + this.state.firstSelectedWord , config)
            .then(res => {
                console.log(res.data);
                this.props.handleSeekTime(res.data);
            })
            .catch(err => {
                console.log(err);
            });
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
                    className="textarea mg-top-30 highlight-text"
                    rows={20} //Would be nice to adapt this to the number of lines in the future
                    onChange={this.handleChange}
                    value={this.state.displayText}
                    onBlur={this.handleBlur}
                />
            </div>
        );
    }
}
