import * as React from 'react';


export class SeeTranscriptionModal extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (

            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card">
                    <div className="modal-container">
                        <header className="modalHeader">
                            <i className="fas fa-edit fontSize2em mg-right-5"></i><p className="modal-card-title whiteText">TEST</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="field">
                                <div className="control">
                                    
                                </div>
                            </div>
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-5" onClick={this.props.onSubmit}>Enregistrer</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
