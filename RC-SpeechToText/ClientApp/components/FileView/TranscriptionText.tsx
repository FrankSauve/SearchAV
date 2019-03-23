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
        if (this.props.textSearch && (prevProps.textSearch !== this.props.textSearch)) {
            this.setState({displayText:this.rawToCleansedHtml(this.state.displayText)});
            this.highlightWords(this.props.selection);
            this.props.handleTextSearch(false);
        }
    }
    
    public highlightWords = (sel: string) =>{
        let s = sel;
        let textArray = this.rawToCleansedHtml(this.state.displayText).split(s);
        let hTextArray = [];
        if(textArray.length !=1) {
            for( let i=0 ; i<textArray.length; i++ ){
                hTextArray.push(textArray[i]);
                if(i!=textArray.length-1){
                    hTextArray.push("<span style='background-color: lightblue'>");
                    hTextArray.push(s);
                    hTextArray.push("</span>");
                }
                
            }
        }
        else{
            //Do nothing since no words are highlighted
        }
        let highlightedText = hTextArray.join("");
        console.log("highlightedText = " + highlightedText);
        console.log("cleansed highlightedText = " + this.rawToCleansedHtml(highlightedText));
        this.setState({displayText: highlightedText});
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
        a = a.replace(/<span[^>]+\>/gi,'');
        return a.replace(/<\/span>/gi,'');
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
                <div 
                    id="transcription" 
                    className="mg-top-30 highlight-text" 
                    contentEditable={true}
                    dangerouslySetInnerHTML={{__html: this.state.displayText}}/>
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
