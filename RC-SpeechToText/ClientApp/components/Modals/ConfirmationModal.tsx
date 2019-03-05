import * as React from 'react';


export class ConfirmationModal extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (

            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card modalCard">
                    <div className="modal-container">
                        <header className="modalHeader">
                            <p className="modal-card-title whiteText">{this.props.title}</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="modalSection">
                                <p>{this.props.confirmMessage}</p>
                            </div>
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-10" onClick={this.props.onConfirm}>{this.props.confirmButton}</button>
                            <button className="button" onClick={this.props.hideModal}>Annuler</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
