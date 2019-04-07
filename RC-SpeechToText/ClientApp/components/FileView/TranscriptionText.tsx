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
    timestamps: string,
    intervalID: any,
    highlightPos: number
}

export class TranscriptionText extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            version: this.props.version,
            displayText: this.rawToHtml(this.props.version.transcription),
            rawText: this.props.version.transcription,
            errorMessage: "",
            timestamps: "0:00:00.00",
            intervalID: null,
            highlightPos: 0
        }
    }
    
    // Called when the component renders
    componentDidMount() {
        
        document.addEventListener('mousedown', this.clearHighlights);

        // Add onBlur and onInput to the contentEditable div
        let transcription = document.querySelector('#transcription');
        if (transcription) {
            transcription.addEventListener('mouseup', this.getHighlightedWords);
            transcription.addEventListener('input', (e) => {this.handleChange(e)});
            transcription.addEventListener('blur', this.handleBlur);
        }
    }
    
    // Remove event listener
    componentWillUnmount() {
        document.addEventListener('mousedown', this.clearHighlights);

        // Remove onBlur and onInput to the contentEditable div
        let transcription = document.querySelector('#transcription');
        if (transcription) {
            transcription.removeEventListener('mouseup', this.getHighlightedWords);
            transcription.removeEventListener('input', this.handleChange);
            transcription.removeEventListener('blur', this.handleBlur);
        }
        if(this.state.intervalID!=null){
            clearInterval(this.state.intervalID);
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
        if (this.props.highlightPosition != null && (prevProps.highlightPosition !== this.props.highlightPosition)) {
            console.log("ACTIVATE TOGGLE TranscriptionText.componentDidUpdate");
            this.setState({highlightPos:this.props.highlightPosition},()=>{
                this.getTimestamps();
            });
        }
        // check if the feature toggle for #75 was deactivated
        else if (this.props.highlightPosition == null && (prevProps.highlightPosition !== this.props.highlightPosition)){
            console.log("DE-ACTIVATE TOGGLE TranscriptionText.componentDidUpdate");
            this.clearPositionHighlights();
        }
    }

    // retrieves all the timestamps from the transcript
    public getTimestamps = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        };
        axios.get('/api/Transcription/GetTimestamps/' + this.state.version.id, config)
            .then(res => {
                this.setState({displayText:this.rawToUnhighlightedPosHtml(this.state.displayText),timestamps: res.data}, () =>{
                    this.highlightPosition();
                });
            })
            .catch(err => {
                console.log(err);
            });
    };
    
    
    // remove all span tags from the page
    public clearHighlights = () =>{
        this.setState({displayText:this.rawToUnhighlightedHtml(this.state.displayText)});
    };
    // remove only span tags dedicated to showing the current position of the video in the transcript
    public clearPositionHighlights=()=>{
        console.log("REACHED TranscriptionText.clearPositionHighlights");
        clearInterval(this.state.intervalID);
        this.setState({displayText:this.rawToUnhighlightedPosHtml(this.state.displayText), intervalID: null});
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
    
    public highlightPosition=()=> {

        let intervalSpeed = 100; // update every intervalSpeed/1000 seconds
        
        console.log("highlightPosition");
        // 0) get displayText in array form (var wordList[] string array of words)
        let wordList = this.state.displayText.split(" ");
        // 1) get list of all timestamps in array form (var timeList[] timestamps converted to seconds)
        let rawTimeList = (this.state.timestamps).split(", ");
        let timeList:number[] = [];
        for(let i=0; i<rawTimeList.length; i++){
            let a = rawTimeList[i].split(':');
            timeList[i] = (+a[0]) * 60 * 60 + (+a[1]) * 60 + (parseFloat(a[2]));
        }
        
        /*
        console.log("highlightPosition words with timelist: \n");
        for(let i=0;i<wordList.length;i++){
            console.log(wordList[i] + " == "+timeList[i] + "\n");
        }
        */
        
        // 2) get current time in video (var currentTime = int form of seconds so far in video)
        console.log("this.props.highlightPosition: "+this.state.highlightPos);
        
        let hTextArray:string[] = [];
        let injected = false;
        let startIndex = 0;
        
        // 3) inject tags around appropriate word
        for(let i=0;i<wordList.length;i++){
            //find first word above currentTime (starting from second)
            if(!injected && timeList[i] >= this.state.highlightPos){
                hTextArray.push("<span style='background-color: #DCDCDC'>");
                injected = true;
                startIndex = i;
            }
            if(wordList[i] !=" " && wordList[i]!="" && wordList[i]!="</span>"){
                hTextArray.push(wordList[i]);
            }
            if(i == (startIndex+1)){
                hTextArray.push("</span>");
            }
        }
        
        console.log("original displayText: \n"+this.state.displayText);

        if(this.state.intervalID == null){
            // 4) call endTag injection method at a given interval
            let interval = setInterval(() => {this.replaceHighlightPosEnd(hTextArray, wordList, timeList, intervalSpeed);}, intervalSpeed);
            this.setState({intervalID:interval});
        }
    };
    
    replaceHighlightPosEnd(hTextArray:string[], wordList:string[], timeList:number[], intervalSpeed:number) {
        console.log("replaceHighlightPosEnd called!");
        
        //if there is nothing left to highlight, clear the highlight and end the process
        if(timeList[timeList.length-1] <=this.state.highlightPos){
            this.clearPositionHighlights();
        }
        
        //clearing away last div close tag
        let spanIndex = hTextArray.indexOf("<span style='background-color: #DCDCDC'>");
        for(let i=spanIndex;i<hTextArray.length;i++){

            if(hTextArray[i] == "</span>"){
                hTextArray.splice(i,1);
                spanIndex = i;
                break;
            }
        }

        for(let i=spanIndex;i<hTextArray.length;i++){

            if(timeList[i] > this.state.highlightPos){
                hTextArray.splice(i,0,"</span>");
                break;
            }
        }
        let str = hTextArray.join(" ");
        console.log("new displayText at time:"+this.state.highlightPos+":\n" + str);
        this.setState({displayText: str, highlightPos: (this.state.highlightPos+(intervalSpeed/1000))});
    }

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
        a = a.replace(/<span[^>]+\>/g,'');
        a = a.replace(/<\/span>/g,'');
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
