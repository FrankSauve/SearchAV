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
                    <header className="modal-card-head">
                        <p className="modal-card-title">Modifier le titre</p>
                        <button className="delete" aria-label="close" onClick={this.props.hideModal}></button>
                    </header>
                    <section className="modal-card-body">
                        <div className="field">
                            <div className="control">
                                <input className="input is-primary" type="text" placeholder={this.props.title ?
                                    (this.props.title.lastIndexOf('.') != -1 ? this.props.title.substring(0, this.props.title.lastIndexOf('.'))
                                        : this.props.title) : "Enter title"} defaultValue={this.props.title} onChange={this.props.handleTitleChange} />
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
