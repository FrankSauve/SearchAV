import * as React from 'react';
import { VideoPlayer } from '../FileView/VideoPlayer';


export class ThumbnailSelectionModal extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public changeThumbnail = () => {

    }

    public render() {
        return (

            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card modalCard">
                    <div className="modal-container">
                        <header className="modalHeader">
                            <p className="modal-card-title whiteText">Choose a thumbnail</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            {this.props.file ? <VideoPlayer path={this.props.file.title} controle={false}/> : null}
                            <input type="range" step="any" min="0" max="1" />
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-5" onClick={this.props.onSubmit}>Accepter</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
