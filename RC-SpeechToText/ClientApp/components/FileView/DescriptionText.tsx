import * as React from 'react';

interface State {
    displayText: string,
    rawText: string
}

export class DescriptionText extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            displayText: this.rawToHtml(this.props.description),
            rawText: this.props.description
        }
    }
    
    // Called when the component renders
    componentDidMount() {
        // Add onBlur and onInput to the contentEditable div
        let description = document.querySelector('#description');
        if (description) {
            description.addEventListener('input', (e) => this.handleChange(e));
            description.addEventListener('blur', this.handleBlur);
        }
    }
    
    // Remove event listener
    componentWillUnmount() {
        // Remove onBlur and onInput to the contentEditable div
        let description = document.querySelector('#description');
        if (description) {
            description.removeEventListener('input', (e) => this.handleChange(e));
            description.removeEventListener('blur', this.handleBlur);
        }
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
        console.log('Returning description:', this.state.rawText);
    };

    public handleChange = (e: any) => {
        let a = e.target.innerHTML.replace(/(?:\r\n|\r|\n)/g, '<br>')
        this.setState({ rawText: a });
        this.setState({ displayText: e.target.innerHTML })
        this.props.handleDescriptionChange(this.state.rawText);

    };

    public render() {
        return (
            <div className="file-view-desc">
                <div
                    id="description"
                    className="file-view-desc-edit"
                    contentEditable={true}
                    dangerouslySetInnerHTML={{ __html: this.props.description}} />
            </div>
        );
    }
}
