import * as React from 'react';
import axios from 'axios';
import auth from '../../Utils/auth';

interface State {
    users: any[],
    unauthorized: boolean
}


export class SelectReviewerModal extends React.Component<any, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            users: [],
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllUsers();
    }

    public getAllUsers = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        }
        axios.get('/api/user/GetAllUsers', config)
            .then(res => {
                console.log(res.data);
                this.setState({ 'users': res.data });
            })
            .catch(err => {
                console.log(err);
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    };

    public render() {
        return (
            <div className={`modal ${this.props.showModal ? "is-active" : null}`} >
                <div className="modal-background"></div>
                <div className="modal-card">
                    <header className="modal-card-head">
                        <p className="modal-card-title">Choisissez un reviseur</p>
                        <button className="delete" aria-label="close" onClick={this.props.hideModal}></button>
                    </header>
                    <section className="modal-card-body">
                        <div className="select is-multiple">
                            <select multiple size={8}>
                                {this.state.users.map((user) => {
                                    {
                                        //Includes current user's name for testing purposes
                                    }
                                    const listUsers = <option value={user.id}>{user.name}</option>
                                    return (
                                        listUsers
                                    )
                                })}
                            </select>
                        </div>
                    </section>
                    <footer className="modal-card-foot">
                        <button className="button is-success">Envoyer la demande</button>
                        <button className="button" onClick={this.props.hideModal}>Annuler</button>
                    </footer>
                </div>
            </div>
        );
    }
}
