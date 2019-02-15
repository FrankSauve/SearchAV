import * as React from 'react'
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    selectedTerms: string
}
export default class HighlightText extends React.Component<any, State> {

    constructor(props: any){
        super(props);
        
        this.state = {
            selectedTerms: ''
        }
    }

    // Add event listener for a click anywhere in the page
    componentDidMount() {
        document.addEventListener('mouseup', this.getHighlightedWords);
    }
    // Remove event listener
    componentWillUnmount() {
        document.removeEventListener('mouseup', this.getHighlightedWords);
    }

    public getHighlightedWords = (event: any) => {
        let s = document.getSelection();
        let selectedWords = s ? s.toString() : null;
        if(selectedWords){
            this.setState({ selectedTerms: selectedWords})
            this.searchTranscript();
        }
    }

    public searchTranscript = () => {
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        console.log(this.state.selectedTerms);
        console.log(this.props.version.id)
        axios.get('/api/TranscriptSearch/SearchTranscript/' + this.props.version.id + '/' + this.state.selectedTerms , config)
            .then(res => {
                console.log(res.data);
            })
            .catch(err => {
                console.log(err);
            });
    }

    public render() {
        return (
            <div className="box highlight-text">
                {this.props.words.map((word: any) => {
                    return (
                        <span key={word.id} data-tag={word.id} className="highlight-text">{word.term} </span>
                    )
                })}
            </div>
        )
    }
}
