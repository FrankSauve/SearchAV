import * as React from 'react';
import { Redirect } from 'react-router-dom';

interface State {
    files: any[],
    usernames: string[],
    loading: boolean,
    unauthorized: boolean
}


export default class ListTable extends React.Component<any, State>
{
    constructor(props: any) {
        super(props);
        this.state = {
            files: this.props.files,
            usernames: this.props.usernames,
            loading: this.props.loading,
            unauthorized: false
        }
    }

    //Will update if props change
    public componentDidUpdate(prevProps: any) {
        if (this.props.files !== prevProps.files) {
            this.setState({ 'files': this.props.files });
        }

        if (this.props.usernames !== prevProps.usernames) {
            this.setState({ 'usernames': this.props.usernames });
        }
    }

    public render() {
        const progressBar = <img src="assets/loading.gif" alt="Loading..." />
        var i = 0;

        return (
            <div>
                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}
                {this.state.loading ? progressBar : null}

                <table>
                    <th>TITRE</th>
                    <th>DUREE</th>
                    <th>IMPORTE PAR</th>
                    <th>DATE DE MODIFICATION</th>
                    <th></th>

                    {this.state.files.map((file) => {

                        i++
                        return (
                            <tr>
                                <td>{file.title}</td>
                                <td>12:00:00</td>
                                <td>{this.state.usernames[i]}</td>
                                <td>29-11-2019 15:00</td>
                                <td>Button</td>
                            </tr>
                        )
                    })}
                </table>
            </div>

            )
    }

}
