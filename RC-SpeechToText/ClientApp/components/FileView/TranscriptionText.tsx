import * as React from 'react';
import { ChangeEvent } from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import * as $ from 'jquery';

interface State {
    version: any,
    displayText: string,
    rawText: string,
    errorMessage: string
}

export class TranscriptionText extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            version: this.props.version,
            displayText: this.rawToHtml(this.props.version.transcription),
            rawText: this.props.version.transcription,
            errorMessage: ""
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
            this.highlightWords();
            this.props.handleTextSearch(false);
        }
        // check if the feature toggle for #75 was activated
        if (this.props.highlightPosition && (prevProps.highlightPosition !== this.props.highlightPosition)) {
            console.log("ACTIVATE TOGGLE TranscriptionText.componentDidUpdate");
            this.highlightPosition(this.props.getTimestamps());
        }
        // check if the feature toggle for #75 was deactivated
        else if (!this.props.highlightPosition && (prevProps.highlightPosition !== this.props.highlightPosition)){
            console.log("DE-ACTIVATE TOGGLE TranscriptionText.componentDidUpdate");
            this.clearPositionHighlights();
        }
    }
    
    // remove all span tags from the page
    public clearHighlights = () =>{
        this.setState({displayText:this.rawToUnhighlightedHtml(this.state.displayText)});
    };
    // remove only span tags dedicated to showing the current position of the video in the transcript
    public clearPositionHighlights=()=>{
        console.log("REACHED TranscriptionText.clearPositionHighlights");
        this.setState({displayText:this.rawToUnhighlightedPosHtml(this.state.displayText)});
    };
    
    // Highlights occurences of a string sel by inserting span tags into the displayText
    public highlightWords = () =>{
        let s = this.props.selection;
        let regex = new RegExp('(\\s|^)'+s+'(\\s|$)','g');
        let textArray = (this.rawToUnhighlightedHtml(this.state.displayText)).split(regex);
        let startTime = "0:00:00.0";
        let endTime = "9:99:99.9";
        let timeOfWordInstances = this.props.timestampInfo.split(", ");
        let hTextArray = [];
        // Iterate over the array of strings gathered from splitting based on sel, and insert span tags between them
        if(textArray.length >1 && this.state.displayText.indexOf(s) != -1) {
            for( let i=0, timeIndex=0; i<textArray.length; i++){
                hTextArray.push(textArray[i]);
                if (i != textArray.length - 1 && textArray[i] != " ") {
                    
                    //get beginning and end timestamps for instance of search term in text
                    let instanceTimeInfo=timeOfWordInstances[timeIndex].toString().split('-');
                    startTime = instanceTimeInfo[0];
                    endTime = instanceTimeInfo[1];
                    timeIndex++;
                    
                    //inject tooltips and highlights into html
                    
                    if(startTime != endTime){
                        hTextArray.push(" <a class='is-primary tooltip is-tooltip-info is-tooltip-active is-tooltip-left' data-tooltip='"+startTime+"'>");
                        hTextArray.push("<a class='is-primary tooltip is-tooltip-info is-tooltip-active is-tooltip-right' data-tooltip='"+endTime+"'>");
                        hTextArray.push("<span style='background-color: #b9e0f9'>");
                        hTextArray.push(s);
                        hTextArray.push("</span>");
                        hTextArray.push("</a>");
                        hTextArray.push("</a>");
                    }
                    else{
                        hTextArray.push(" <a class='is-primary tooltip is-tooltip-info is-tooltip-active is-tooltip-left' data-tooltip='"+startTime+"'>");
                        hTextArray.push("<span style='background-color: #b9e0f9'>");
                        hTextArray.push(s);
                        hTextArray.push("</span>");
                        hTextArray.push("</a>");
                    }
                }
                
            }
            let highlightedText = hTextArray.join("");
            this.setState({displayText: highlightedText});
        }
    };
    
    public highlightPosition=(timestamps: string)=> {

        console.log("highlightPosition");
        //TODO: IMPLEMENT METHOD
        // Ideas for implementation: 

        // DECENT IDEA
        //-1) Write div shift method to make the highlight shift one word over each time it is called.
        // TODO: Write this method
        
        // 0) get displayText in array form (var wordList[] string array of words)
        let wordList = this.state.displayText.split(" ");
        console.log("highlightPosition wordList:" + wordList.toString());
        // 1) get list of all timestamps in array form (var timeList[] timestamps converted to seconds)
        console.log("highlightPosition timestamps:" + timestamps);
        
        let rawTimeList = (timestamps).split(", ");
        console.log("highlightPosition rawTimeList:" + rawTimeList.toString());
        let timeList = [];
        
        for(let i=0; i<rawTimeList.length; i++){
            let a = rawTimeList[i].split(':');
            timeList[i] = (+a[0]) * 60 * 60 + (+a[1]) * 60 + (parseFloat(a[2]));
        }
        console.log("highlightPosition timeList: "+timeList.toString());
        
        // 2) get current time in video (var currentTime = int form of seconds so far in video)
        
        
        console.log("IMPLEMENT TranscriptionText.highlightPosition");
    };

    // Retrieves the words selected via mouse and calls FileView's searchTranscript method with them
    public getHighlightedWords = (event: any) => {
        let s = document.getSelection();
        let selectedWords = s ? s.toString().split(" ") : null;
        
        let str = s.toString().trim();
        
        if (selectedWords && s && str) {
            this.props.handleSelectionChange(str);
        }
        
    };

    // removes all div tags from a string
    rawToUnhighlightedPosHtml(text: string) {
        console.log("REACHED TranscriptionText.rawToUnhighligtedPosHtml");
        let a = text;
        a = a.replace(/<div[^>]+\>/g,''); //<div style:'background-color: #DCDCDC'>
        a = a.replace(/<\/div>/g,'');
        return a;
    }
    
    // removes all span and a tags from a string
    rawToUnhighlightedHtml(text: string) {
        let a = text;
        a = a.replace(/<span[^>]+\>/g,'');
        a = a.replace(/<\/span>/g,'');
        a = a.replace(/<a[^>]+\>/g,'');
        a = a.replace(/<\/a>/g,'');
        return a;
    }

    // removes all br tags from a string
    rawToHtml(text: string) {
        return text.replace(/&nbsp;/gi, "\n");
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
        let el = document.getElementById("transcription");
        let range = document.createRange();
        let sel = window.getSelection();
        if(el) range.setStart(el.childNodes[0], cursorPos);
        range.collapse(true);
        sel.removeAllRanges();
        sel.addRange(range);
    };

    getCursorPosition = () => {
        let sel = document.getSelection();
        if(sel) {
            let range = sel.getRangeAt(0);
            return range.startOffset;
        }
    };

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
