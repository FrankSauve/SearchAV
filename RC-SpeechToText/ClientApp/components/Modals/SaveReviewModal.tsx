import * as React from 'react';
import auth from '../../Utils/auth';
import axios from 'axios';
import { ErrorModal } from './ErrorModal';
import { SuccessModal } from './SuccessModal';
import { EventHandler, ChangeEvent } from 'react';

interface State {
    fileId: any,
    errorMessage: string,
    showSuccessModal: boolean,
    showErrorModal: boolean,
    unauthorized: boolean,
}


    export class SaveReviewModal extends React.Component<any, State>
    {
        constructor(props: any) {
            super(props);

            this.state = {
                fileId: 0,
                errorMessage: "",
                showSuccessModal: false,
                showErrorModal: false,
                unauthorized: false,
            }
        }

        public showSuccessModal = () => {
            this.setState({ showSuccessModal: true });
        }

        public hideSuccessModal = () => {
            this.setState({ showSuccessModal: false });
        }

        public hideErrorModal = () => {
            this.setState({ showErrorModal: false });
        }

        // Called when the component gets rendered
        public componentDidMount() {
            var id = window.location.href.split('/')[window.location.href.split('/').length - 1]; //Getting fileId from url
            this.setState({ fileId: id });
        }

        public render() {
            return (
                <div className={`modal ${this.props.showModal ? "is-active" : null}`} >

                    <ErrorModal
                        showModal={this.state.showErrorModal}
                        hideModal={this.hideErrorModal}
                        title="Courriel non envoyé"
                        errorMessage={this.state.errorMessage}
                    />

                    <SuccessModal
                        showModal={this.state.showSuccessModal}
                        hideModal={this.hideSuccessModal}
                        title="Sauveguarde de fichier"
                        successMessage="Courriel envoyé"
                    />

                    <div></div>
                    <div>
                        <div>
                            <header>
                                <p>{this.props.title}</p>
                                <button aria-label="close" onClick={this.props.hideModal} ></button>
                            </header>
                            <footer>
                                <button>Confirmer</button>
                            </footer>
                        </div>
                    </div>
                </div>
            );
        }

    }

