import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';
import HighlightText from './HighlightText';

interface State {
    words: any
}
export class TranscriptionText extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
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

    public render() {
        return (
            <div>
                <textarea
                    className="textarea mg-top-30"
                    rows={20} //Would be nice to adapt this to the number of lines in the future
                    defaultValue={this.props.text}
                    onChange={this.props.handleChange}
                />
                {this.state.words ? <HighlightText version={this.props.version} words={this.state.words} /> : null}
            </div>
        );
    }
}
