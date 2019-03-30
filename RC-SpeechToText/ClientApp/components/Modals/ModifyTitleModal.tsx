import * as React from 'react';


export class ModifyTitleModal extends React.Component<any> {
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
                            <i className="fas fa-edit fontSize2em mg-right-5"></i><p className="modal-card-title whiteText">Modifier le titre</p>
                            <button className="delete" aria-label="close closeModal" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="field">
                                <div className="control">
                                    <input className="input is-primary" type="text" placeholder={this.props.title ?
                                        (this.props.title.lastIndexOf('.') != -1 ? this.props.title.substring(0, this.props.title.lastIndexOf('.'))
                                        : this.props.title) : "Entrer le titre"} defaultValue={this.props.title} onChange={this.props.handleTitleChange} />
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
