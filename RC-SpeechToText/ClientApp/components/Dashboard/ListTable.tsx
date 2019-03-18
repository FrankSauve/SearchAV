import * as React from 'react';
import { Redirect } from 'react-router-dom';
import TableImage from './TableImage';
import { Button } from './Button';

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

                <table className='table is-fullwidth'>
                    <th>TITRE</th>
                    <th>IMPORTE PAR</th>
                    <th>DATE DE MODIFICATION</th>
                    <th></th>

                    {this.state.files.map((file) => {

                        i++
                        return (
                            <tr>
                                <td width='100'>
                                    <TableImage
                                        fileId={file.id}
                                        thumbnailPath={file.thumbnailPath == "NULL" ? "assets/audioIcon.png" : file.thumbnailPath}
                                        title={file.title}
                                        flag={file.flag}
                                        description={file.description} />
                                </td>
                                <td>{this.state.usernames[i]}</td>
                                <td>{file.dateAdded.substring(0, 10) + " " + file.dateAdded.substring(11, 16)}</td>
                                <td>
                                    <Button
                                        fileId={file.id}
                                        title={file.title}
                                        description={file.description}
                                        flag={file.flag}
                                        username={this.state.usernames[i]}
                                        image={file.type == "Audio" ? 'assets/speakerIcon.png' : file.thumbnailPath}
                                        date={file.dateAdded.substring(0, 10) + " " + file.dateAdded.substring(11, 16)}
                                    />
                                </td>
                            </tr>
                        )
                    })}
                </table>
            </div>
        )
    }

}
