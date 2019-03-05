import * as React from 'react';


export class ErrorModal extends React.Component<any> {
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
                            <i className="fas fa-exclamation-triangle mg-right-10"></i><p className="modal-card-title whiteText">{this.props.title}</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="modalSection padding-bottom-10">
                                <p>{this.props.errorMessage}</p>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        );
    }
}
