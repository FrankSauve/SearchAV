import * as React from 'react';


export class ModifyDescriptionModal extends React.Component<any> {
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
                            <i className="fas fa-edit fontSize2em mg-right-5"></i><p className="modal-card-title whiteText">{this.props.description && this.props.description != "" ? "Modifier la description" : "Ajouter une description"}</p>
                            <button className="delete closeModal" aria-label="close" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="field">
                                <div className="control">
                                    <textarea className="textarea is-primary" type="text" placeholder={this.props.description && this.props.description != "" ?
                                        this.props.description : "Enter description"} defaultValue={this.props.description ? this.props.description : ""}
                                        onChange={this.props.handleDescriptionChange} />
                                </div>
                            </div>
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-success mg-right-5" onClick={this.props.onSubmit}>Enregistrer</button>
                            <button className="button" onClick={this.props.hideModal}>Annuler</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
