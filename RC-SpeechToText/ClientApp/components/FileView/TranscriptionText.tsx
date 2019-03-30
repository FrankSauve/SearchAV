import * as React from 'react';
import { ChangeEvent } from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import * as $ from 'jquery';

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
        document.addEventListener('mousedown', this.clearHighlights);

        // Add onBlur and onInput to the contentEditable div
        let transcription = document.querySelector('#transcription');
        if (transcription){
            transcription.addEventListener('input', (e) => {this.handleChange(e)});
            transcription.addEventListener('blur', this.handleBlur);
        }
    }
    
    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.getHighlightedWords);
        document.addEventListener('mousedown', this.clearHighlights);
        
        // Remove onBlur and onInput to the contentEditable div
        let transcription = document.querySelector('#transcription');
        if (transcription){
            transcription.removeEventListener('input', this.handleChange);
            transcription.removeEventListener('blur', this.handleBlur);
        }
    }

    componentDidUpdate(prevProps : any, prevState : any) {
        // check if the button on TranscriptionSearch was pressed
        if (this.props.textSearch && (prevProps.textSearch !== this.props.textSearch)) {
            this.clearHighlights();
            this.highlightWords(this.props.selection);
            this.props.handleTextSearch(false);
        }
    }
    
    // remove all span tags from the page
    public clearHighlights = () =>{
        this.setState({displayText:this.rawToUnspannedHtml(this.state.displayText)});
    };
    
    // Highlights occurences of a string sel by inserting span tags into the displayText
    public highlightWords = (sel: string) =>{
        let s = sel;
        let textArray = this.rawToUnspannedHtml(this.state.displayText).split(s);
        let hTextArray = [];
        // Iterate over the array of strings gathered from splitting based on sel, and insert span tags between them
        if(textArray.length !=1 && textArray.length !=0 && this.state.displayText.indexOf(s) != -1) {
            for( let i=0 ; i<textArray.length; i++ ){
                hTextArray.push(textArray[i]);
                if(i!=textArray.length-1){
                    hTextArray.push("<span style='background-color: #b9e0f9'>");
                    hTextArray.push(s);
                    hTextArray.push("</span>");
                }
                
            }
            let highlightedText = hTextArray.join("");
            this.setState({displayText: highlightedText});
        }
    };

    // Retrieves the words selected via mouse and calls FileView's searchTranscript method with them
    public getHighlightedWords = (event: any) => {
        let s = document.getSelection();
        let selectedWords = s ? s.toString().split(" ") : null;
        if (selectedWords && s) {
            this.props.handleSelectionChange(s.toString());

            //Get first non-empty string element
            for (var i = 0; i < selectedWords.length; i++) {
                if (selectedWords[i].localeCompare("") != 0) {
                    this.setState({ firstSelectedWord: selectedWords[i] });
                    break;
                }
            }
            //call the search func if there is at least one word selected
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

    // removes all span tags from a string
    rawToUnspannedHtml(text: string) {
        let a = text;
        a = a.replace(/<span[^>]+\>/g,'');
        return a.replace(/<\/span>/g,'');
    }

    // removes all br tags from a string
    rawToHtml(text: string) {
        return text.replace(/<br\s*[\/]?>/gi, "\n");
    }

    public handleBlur = () => {
        console.log('Returning:', this.state.rawText);
        
    };

    public handleChange = (e: any) => {
        let cursorPos:any = this.getCursorPosition();
        let a = e.target.innerHTML.replace(/(?:\r\n|\r|\n)/g, '<br>')
        this.setState({ rawText: a });
        this.setState({ displayText: e.target.innerHTML })
        console.log(this.state.rawText);
        this.props.handleTranscriptChange(this.state.rawText);
        this.setCursorPosition(cursorPos);

    };

     setCursorPosition = (cursorPos: number) => {
        var el = document.getElementById("transcription");
        var range = document.createRange();
        var sel = window.getSelection();
        if(el) range.setStart(el.childNodes[0], cursorPos);
        range.collapse(true);
        sel.removeAllRanges();
        sel.addRange(range);
    }

    getCursorPosition = () => {
        var sel = document.getSelection();
        if(sel) {
            var range = sel.getRangeAt(0);
            return range.startOffset;
        }
    }

    public render() {
        return (
            <div>
                <div 
                    id="transcription" 
                    className="highlight-text" 
                    contentEditable={true}
                    dangerouslySetInnerHTML={{__html: this.state.displayText}}/>
            </div>
        );
    }
}
