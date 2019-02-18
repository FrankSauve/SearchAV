import * as React from 'react';


export class SuccessModal extends React.Component<any> {
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
                        <p className="has-text-success">{this.props.successMessage}</p>
                    </section>
                    <footer className="modal-card-foot">
                    </footer>
                </div>
            </div>
        );
    }
}
