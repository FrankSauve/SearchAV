import 'bootstrap';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter, Link } from 'react-router-dom';
import * as RoutesModule from '../../routes';
import { RouteComponentProps } from 'react-router';
let routes = RoutesModule.routes;

interface GetFileDataState {
empList: FileData[];
loading: boolean;
} 

export class GetFile extends React.Component<RouteComponentProps<{}>, GetFileDataState>
{
    constructor(props: any) {
    super(props);
    this.state = { empList: [], loading: true };

    fetch('api/FileController/GetAllFiles')
        .then(response => response.json() as Promise<FileData[]>)
        .then(data => {
            this.setState({ empList: data, loading: false });
        });

    this.handleDelete = this.handleDelete.bind(this);

}

public render() {
    let contents = this.state.loading
        ? <p><em>Loading...</em></p>
        : this.renderFileTable(this.state.empList);

    return <div>
        <h1>File Data</h1>
        <p>All Files</p>
        <p>
            <Link to="/addFile">Create New</Link>
        </p>
        {contents}
    </div>;
} 

private handleDelete(id: number) {
    if (!confirm("Do you want to delete File with Id: " + id))
        return;
    else {
        fetch('api/File/Delete/' + id, {
            method: 'delete'
        }).then(data => {
            this.setState(
                {
                    empList: this.state.empList.filter((rec) => {
                        return (rec.fileId != id);
                    })
                });
        });
    }
} 

private renderFileTable(empList: FileData[]) {
    return <table className='table'>
        <thead>
            <tr>
                <th></th>
                <th>FileId</th>
                <th>Name</th>
                <th>Path</th>
            </tr>
        </thead>
        <tbody>
            {empList.map(fil =>
                <tr key={fil.fileId}>
                    <td></td>
                    <td>{fil.fileId}</td>
                    <td>{fil.name}</td>
                    <td>{fil.path}</td>
                    <td>
                        <a className="action" onClick={(id) => this.handleDelete(fil.fileId)}>Delete</a>
                    </td>
                </tr>
            )}
        </tbody>
    </table>;
}
} 

export class FileData {
fileId: number = 0;
name: string = "";
path: string = "";
}


