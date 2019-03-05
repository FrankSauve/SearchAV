import * as React from 'react';


export class ConfirmationModal extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (

            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card">
                    <header className="modal-card-head">
                        <p className="modal-card-title">{this.props.title}</p>
                        <button className="delete" aria-label="close" onClick={this.props.hideModal}></button>
                    </header>
                    <section className="modal-card-body">
                        <p>{this.props.confirmMessage}</p>
                    </section>
                    <footer className="modal-card-foot">
                        <button className="button is-success" onClick={this.props.onConfirm} disabled={this.props.disabled}>{this.props.confirmButton}</button>
                        <button className="button" onClick={this.props.hideModal}>Annuler</button>
                    </footer>
                </div>
            </div>
        );
    }
}
