import * as React from 'react';


export class AddTitleDescriptionModal extends React.Component<any> {
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
                            <i className="fas fa-edit fontSize2em mg-right-5"></i><p className="modal-card-title whiteText"> Ajouter un titre et une description </p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="field">
                                <input className="input is-primary" type="text" placeholder="Choisir un titre pour le fichier" />
                            </div>
                            <div className="field">
                                <div className="control">
                                    <textarea className="textarea is-primary" type="text" placeholder="Entrez une description pour le fichier. (optionnel)"
                                        onChange={this.props.handleDescriptionChange} />
                                </div>
                            </div>
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-5" onClick={this.props.onSubmit}>Transcrire le fichier</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
