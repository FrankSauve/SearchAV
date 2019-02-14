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
                    <header className="modal-card-head">
                        <p className="modal-card-title">{this.props.description && this.props.description != "" ? "Modifier la description" : "Ajouter une description"}</p>
                        <button className="delete" aria-label="close" onClick={this.props.hideModal}></button>
                    </header>
                    <section className="modal-card-body">
                        <div className="field">
                            <div className="control">
                                <textarea className="textarea is-primary" type="text" placeholder={this.props.description && this.props.description != "" ?
                                    this.props.description : "Enter description"} defaultValue={this.props.description ? this.props.description : ""}
                                    onChange={this.props.handleDescriptionChange} />
                            </div>
                        </div>
                    </section>
                    <footer className="modal-card-foot">
                        <button className="button is-success" onClick={this.props.onSubmit}>Enregistrer</button>
                        <button className="button" onClick={this.props.hideModal}>Annuler</button>
                    </footer>
                </div>
            </div>
        );
    }
}
