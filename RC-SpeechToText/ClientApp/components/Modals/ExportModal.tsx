import * as React from 'react';


export class ExportModal extends React.Component<any> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return (
            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card exportModalCard">
                    <div className="export-modal-container">
                        <header className="exportModalHeader">
                        <p className="modal-card-title whiteText">{this.props.title}</p>
                            <button className="delete exportCloseModal" aria-label="close" onClick={this.props.hideModal} ></button>
                    </header>
                        <section className="exportModalBody">
                            <div className="exportSection">
                                <p className="exportModalLabel">Format de fichier :</p>
                            </div>
                            <div className="exportSection">
                                <select className="exportModalDropdown">
                                    <option value="0">Choisissez un format</option>   
                                    <option value="doc">.DOC</option>
                                    <option value="srt">.SRT</option>
                                    <option value="googleDoc">Google Doc</option>
                                 </select>
                            </div>
                            <input type="checkbox" value="dw" className="exportCheckBox"></input><span>Incrustrer les sous-titres sur la vidéo</span>
                    </section>
                        <footer className="exportModalFooter">
                        <button className="button is-success pull-right" onClick={this.props.onConfirm}>Confirmer</button>
                        </footer>
                    </div>
                </div>
            </div>
        );
    }
}
