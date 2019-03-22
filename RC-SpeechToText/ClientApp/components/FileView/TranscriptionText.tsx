import * as React from 'react';
import { ChangeEvent } from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    version: any,
    displayText: string,
    rawText: string,
    errorMessage: string,
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

        // Add onBlur and onInput to the contentEditable div
        let transcription = document.querySelector('#transcription');
        if (transcription){
            transcription.addEventListener('input', (e) => this.handleChange(e));
            transcription.addEventListener('blur', this.handleBlur);
        }
    }

    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.getHighlightedWords);
        // Remove onBlur and onInput to the contentEditable div
        let transcription = document.querySelector('#transcription');
        if (transcription){
            transcription.removeEventListener('input', (e) => this.handleChange(e));
            transcription.removeEventListener('blur', this.handleBlur);
        }
    }

    componentDidUpdate(prevProps : any, prevState : any) {
        // only call for the change in time if the data has changed
        if (prevProps.selection !== this.props.selection) {
            this.highlightWords();
        }
    }
    
    public highlightWords = () =>{
        //this.setState({displayText : this.rawToCleansedHtml(this.props.version.transcription)});
        let s1 = this.rawToCleansedHtml(this.props.version.transcription);
        let s = "tribunaux";
        let str = this.state.displayText.split(s);
        let str2 = [];
        if(str.length !=1) {
            for( let i=0 ; i<str.length; i++ ){
                str2.push(str[i]);
                str2.push("<span className='highlight'>");
                str2.push(s);
                str2.push("</span>");
            }
        }
        else{
            //Do nothing since no words are highlighted
        }
        let s2 = str2.join("");
        console.log("s2 = " + s2);
    };

    public getHighlightedWords = (event: any) => {
        let s = document.getSelection();
        let selectedWords = s ? s.toString().split(" ") : null;
        if (selectedWords && s) {

            this.props.handleSelectionChange(s.toString());
            //this.setState({selection: s.toString()});

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
                this.props.searchTranscript(this.props.selection, false);
            }
        }
    };

    rawToCleansedHtml(text: string) {
        let a = this.rawToHtml(text);
        return a.replace(/<span[^>]+\>/i,'');
    }

    rawToHtml(text: string) {
        return text.replace(/<br\s*[\/]?>/gi, "\n");
    }

    public handleBlur = () => {
        console.log('Returning:', this.state.rawText);
        this.props.handleChange(this.state.rawText)
    };

    public handleChange = (e: any) => {
        let a = e.target.value.replace(/(?:\r\n|\r|\n)/g, '<br>')
        this.setState({ rawText: a });
        this.setState({ displayText: e.target.value })
    };
    
    

    public render() {
        return (
            <div>
                <div id="transcription" className="mg-top-30 highlight-text" contentEditable={true}>
                    {this.state.displayText}
                    
                </div>
                {/* <textarea
                    className="textarea mg-top-30 highlight-text"
                    rows={20} //Would be nice to adapt this to the number of lines in the future
                    onChange={this.handleChange}
                    value={this.state.displayText}
                    onBlur={this.handleBlur}
                /> */}
            </div>
        );
    }
}
