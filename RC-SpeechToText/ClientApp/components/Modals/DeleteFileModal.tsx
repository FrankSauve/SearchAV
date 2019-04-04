import * as React from 'react';


export class DeleteFileModal extends React.Component<any> {
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
                            <i className="fas fa-trash fontSize2em mg-right-5"></i><p className="modal-card-title whiteText">Effacer le fichier</p>
                            <button className="delete" aria-label="close closeModal" onClick={this.props.hideModal}></button>
                        </header>
                        <section className="modalBody">
                            <div className="modalSection padding-bottom-10">
                                <p>Êtes-vous sûr de vouloir supprimer ce fichier? Cette action est irréversible!</p>
                            </div>
                        </section>
                        <footer className="modalFooter">
                            <button className="button is-danger mg-right-5" onClick={this.props.onSubmit}>Effacer</button>
                            <button className="button" onClick={this.props.hideModal}>Annuler</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
